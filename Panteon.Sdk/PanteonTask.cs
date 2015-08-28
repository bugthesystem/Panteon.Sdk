using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Panteon.Sdk.Configuration;
using Panteon.Sdk.Models;
using Schyntax;
using StackExchange.Redis;

namespace Panteon.Sdk
{
    public abstract class PanteonTask : IPanteonTask
    {
        protected ILogger Logger { get; private set; }
        protected ITaskSettings TaskSettings { get; private set; }

        protected IConnectionMultiplexer Multiplexer { get; private set; }
        public RedisSchtickWrapper Wrapper { get; private set; }

        private readonly Schtick _schtick;

        protected PanteonTask(ILogger logger, ITaskSettings taskSettings)
        {
            Logger = logger;
            TaskSettings = taskSettings;
            _schtick = new Schtick();
            Multiplexer = ConnectionMultiplexer.Connect(TaskSettings.RedisConnectionString);
            Wrapper = new RedisSchtickWrapper(() => Multiplexer.GetDatabase(TaskSettings.DbNo));
        }

        protected ScheduledTask ScheduledTask { get; set; }
        public abstract string Name { get; }

        public abstract bool Bootstrap(bool startImmediately);

        public virtual bool Run(Action<ScheduledTask, DateTimeOffset> actionToRun, bool autoRun = true)
        {
            Console.WriteLine($"{Name} is started.");

            try
            {
                Logger.Info($"{Name} is started.");
                //TODO: async

                ScheduledTask = _schtick.AddAsyncTask(Name, TaskSettings.SchedulePattern,
                    Wrapper.WrapAsync(async (task, timeIntendedToRun) =>
                        await Task.Run(() => actionToRun?.Invoke(task, timeIntendedToRun)).ConfigureAwait(false))
                    , autoRun);

                ScheduledTask.OnException += ScheduledTask_OnException;

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error($"An error occurrred while executing {Name}", exception);
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
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error("", exception);
                return false;
            }
        }

        public bool Start(DateTimeOffset lastKnownEvent = default(DateTimeOffset))
        {
            if (!ScheduledTask.IsScheduleRunning)
            {
                ScheduledTask.StartSchedule(lastKnownEvent);
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
        }

        private void ScheduledTask_OnException(ScheduledTask arg1, Exception arg2)
        {
            Logger.Error($"An error occurred while executing {arg1.Name}", arg2);
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