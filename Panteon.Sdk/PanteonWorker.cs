using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Panteon.Sdk.Configuration;
using Panteon.Sdk.Events;
using Panteon.Sdk.History;
using Panteon.Sdk.Models;
using Schyntax;
using StackExchange.Redis;

namespace Panteon.Sdk
{
    public abstract class PanteonWorker : IPanteonWorker
    {
        protected ILogger WorkerLogger { get; private set; }
        protected IHistoryStorage HistoryStorage { get; private set; }
        protected IWorkerSettings WorkerSettings { get; private set; }

        protected event EventHandler<WorkerStartedEventArgs> OnStarted;
        protected event EventHandler<WorkerStoppedEventArgs> OnStopped;
        protected event EventHandler<WorkerPausedEventArgs> OnPaused;

        protected event EventHandler<TaskExceptionEventArgs> OnTaskException;
        protected event EventHandler<WorkerEventArgs> OnTaskEnter;
        protected event EventHandler<WorkerEventArgs> OnTaskExit;

        protected IConnectionMultiplexer Multiplexer { get; private set; }
        protected RedisSchtickWrapper TaskWrapper { get; private set; }

        private readonly Schtick _schtick;

        protected PanteonWorker(ILogger workerLogger, IWorkerSettings workerSettings, IHistoryStorage historyStorage)
        {
            HistoryStorage = historyStorage;
            WorkerLogger = workerLogger;
            WorkerSettings = workerSettings;
            _schtick = new Schtick();

            Multiplexer = ConnectionMultiplexer.Connect(WorkerSettings.RedisConnectionString);
            TaskWrapper = new RedisSchtickWrapper(() => Multiplexer.GetDatabase(WorkerSettings.DbNo));
        }

        protected ScheduledTask ScheduledTask { get; set; }
        public abstract string Name { get; }

        public abstract bool Init(bool autoRun);

        public virtual bool Run(Action<ScheduledTask, DateTimeOffset> actionToRun, bool autoRun = true)
        {


            try
            {
                string message = $"{Name} is started.";
                WorkerLogger.Info(message);
                Store(message);

                ScheduledTask = _schtick.AddAsyncTask(Name, WorkerSettings.SchedulePattern,
                    TaskWrapper.WrapAsync(async (task, timeIntendedToRun) =>
                        await Task.Run(() =>
                        {
                            OnTaskEnter?.Invoke(this, new WorkerEventArgs());

                            actionToRun?.Invoke(task, timeIntendedToRun);

                            OnTaskExit?.Invoke(this, new WorkerEventArgs());
                        }).ConfigureAwait(false))
                    , autoRun);

                ScheduledTask.OnException += ScheduledTask_OnException;

                OnStarted?.Invoke(this, new WorkerStartedEventArgs());

                return true;
            }
            catch (Exception exception)
            {
                string details = $"An error occurrred while executing {Name}";
                WorkerLogger.Error(details, exception);
                Store($"{details}, Exception = {exception.Message}");
                return false;
            }
        }

        public bool Update(ScheduleInfo scheduleInfo)
        {
            ScheduledTask.UpdateSchedule(new Schedule(scheduleInfo.ScheduleExpression));

            return true;
        }

        public virtual PanteonTaskInfo Inspect()
        {
            if (ScheduledTask != null)
            {
                return new PanteonTaskInfo
                {
                    Name = ScheduledTask.Name,
                    IsScheduleRunning = ScheduledTask.IsScheduleRunning,
                    NextEvent = ScheduledTask.NextEvent,
                    PrevEvent = ScheduledTask.PrevEvent
                };
            }

            return null;
        }


        public virtual bool Stop()
        {
            try
            {
                ScheduledTask.StopSchedule();

                OnStopped?.Invoke(this, new WorkerStoppedEventArgs());

                return true;
            }
            catch (Exception exception)
            {
                WorkerLogger.Error("", exception);
                return false;
            }
        }

        public bool Start(DateTimeOffset lastKnownEvent = default(DateTimeOffset))
        {
            if (!ScheduledTask.IsScheduleRunning)
            {
                ScheduledTask.StartSchedule(lastKnownEvent);

                OnStarted?.Invoke(this, new WorkerStartedEventArgs());

                return true;
            }

            return false;
        }

        public virtual void Pause(TimeSpan duration)
        {
            //TODO: pause
            var now = DateTime.Now;
            var nextStartDate = now.AddSeconds(duration.Seconds);

            ScheduledTask.StopSchedule();

            OnPaused?.Invoke(this, new WorkerPausedEventArgs());
        }

        private void ScheduledTask_OnException(ScheduledTask task, Exception exception)
        {
            string message = $"An error occurred while executing {task.Name}";

            WorkerLogger.Error(message, exception);
            Store($"{message}, Exception = {exception.Message}");

            OnTaskException?.Invoke(this, new TaskExceptionEventArgs(task, exception));
        }

        public virtual void Progress(ProgressMessage message)
        {
            if (message != null)
            {
                string format = $"{Name} task propress update Message: {message.Message}, Percent : {message.Percent}";
                Console.WriteLine(format);
                Store(format);
            }
        }

        public virtual IEnumerable<HistoryModel> LoadHistory(DateTime? @from = null, DateTime? to = null)
        {
            return HistoryStorage.Load(Name, from, to);
        }

        public void Dispose()
        {
            ScheduledTask.StopSchedule();
        }

        private void Store(string message)
        {
            HistoryStorage.Store(new HistoryModel { DateCreated = DateTime.Now, Details = message, Name = Name });
        }
    }
}