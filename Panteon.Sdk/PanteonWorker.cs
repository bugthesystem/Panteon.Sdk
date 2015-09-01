using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Panteon.Sdk.Configuration;
using Panteon.Sdk.Events;
using Panteon.Sdk.Models;
using Schyntax;
using StackExchange.Redis;

namespace Panteon.Sdk
{
    public abstract class PanteonWorker : IPanteonWorker
    {
        protected ILogger WorkerLogger { get; private set; }
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

        protected PanteonWorker(ILogger workerLogger, IWorkerSettings workerSettings)
        {
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
            Console.WriteLine($"{Name} is started.");

            try
            {
                WorkerLogger.Info($"{Name} is started.");

                //TODO: Async
                //TODO: Reafctor & Improve

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
                WorkerLogger.Error($"An error occurrred while executing {Name}", exception);
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
            var now = DateTime.Now;
            var nextStartDate = now.AddSeconds(duration.Seconds);

            ScheduledTask.StopSchedule();
            //TODO: pause

            OnPaused?.Invoke(this, new WorkerPausedEventArgs());
        }

        private void ScheduledTask_OnException(ScheduledTask arg1, Exception arg2)
        {
            WorkerLogger.Error($"An error occurred while executing {arg1.Name}", arg2);

            OnTaskException?.Invoke(this, new TaskExceptionEventArgs(arg1, arg2));
        }

        public virtual void Progress(ProgressMessage message)
        {
            if (message != null)
                Console.WriteLine($"{Name} task propress update {nameof(message.Message)}: {message.Message}, {nameof(message.Percent)} : {message.Percent}");
        }

        public void Dispose()
        {
            ScheduledTask.StopSchedule();
        }
    }
}