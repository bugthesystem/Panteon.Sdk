using System;
using Autofac.Extras.NLog;
using Panteon.Sdk.Configuration;
using Panteon.Sdk.Events;
using Panteon.Sdk.Models;
using Panteon.Sdk.Realtime;

namespace Panteon.Sdk
{
    public abstract class RealtimePanteonWorker : PanteonWorker
    {
        protected IPubSubClient PubSubClient { get; private set; }

        protected RealtimePanteonWorker(ILogger workerLogger, IWorkerSettings workerSettings, IPubSubClient pubSubClient) : base(workerLogger, workerSettings)
        {
            PubSubClient = pubSubClient;

            OnStarted += Worker_Started;
            OnPaused += Worker_Paused;
            OnStopped += Worker_Stopped;

            OnTaskException += Task_OnTaskException;
            OnTaskEnter += Task_OnEnter;
            OnTaskExit += Task_OnExit;
        }

        private void Worker_Started(object sender, WorkerStartedEventArgs e)
        {
            try
            {
                IPubSubResult result = PubSubClient.Publish(new PubSubMessage
                {
                    Event = "task:started",
                    Channel = "panteon",
                    Payload = new
                    {
                        TaskName = Name
                    }
                });
                Console.WriteLine(result.Body);
            }
            catch (Exception exception)
            {
                WorkerLogger.Error($"An error occurred while informing about [{Name}] starting operation. ", exception);
            }
        }


        private void Worker_Paused(object sender, WorkerPausedEventArgs workerPausedEventArgs)
        {
            try
            {
                IPubSubResult result = PubSubClient.Publish(new PubSubMessage
                {
                    Event = "task:onpaused",
                    Channel = "panteon",
                    Payload = new
                    {
                        TaskName = Name
                    }
                });

                Console.WriteLine(result.Body);
            }
            catch (Exception exception)
            {
                WorkerLogger.Error($"An error occurred while informing about [{Name}] pausing operation. ", exception);
            }
        }

        private void Worker_Stopped(object sender, WorkerStoppedEventArgs workerStoppedEventArgs)
        {
            try
            {
                IPubSubResult result = PubSubClient.Publish(new PubSubMessage
                {
                    Event = "task:onstopped",
                    Channel = "panteon",
                    Payload = new
                    {
                        TaskName = Name
                    }
                });

                Console.WriteLine(result.Body);
            }
            catch (Exception exception)
            {
                WorkerLogger.Error($"An error occurred while informing about [{Name}] stopping operation. ", exception);
            }
        }

        public override void Progress(ProgressMessage message)
        {
            message.TaskName = Name;

            IPubSubResult result = PubSubClient.Publish(new PubSubMessage
            {
                Event = "task:onprogress",
                Channel = "panteon",
                Payload = message
            });

            Console.WriteLine(result.Body);
        }

        private void Task_OnEnter(object sender, WorkerEventArgs e)
        {
            try
            {
                IPubSubResult result = PubSubClient.Publish(new PubSubMessage
                {
                    Event = "task:onenter",
                    Channel = "panteon",
                    Payload = new
                    {
                        TaskName = Name
                    }
                });
                Console.WriteLine(result.Body);
            }
            catch (Exception exception)
            {
                WorkerLogger.Error($"An error occurred while informing about [{Name}] action enter. ", exception);
            }
        }

        private void Task_OnExit(object sender, WorkerEventArgs e)
        {
            try
            {
                IPubSubResult result = PubSubClient.Publish(new PubSubMessage
                {
                    Event = "task:onexit",
                    Channel = "panteon",
                    Payload = new
                    {
                        TaskName = Name
                    }
                });
                Console.WriteLine(result.Body);
            }
            catch (Exception exception)
            {
                WorkerLogger.Error($"An error occurred while informing about [{Name}] action exit. ", exception);
            }
        }


        private void Task_OnTaskException(object sender, TaskExceptionEventArgs e)
        {
            try
            {
                IPubSubResult result = PubSubClient.Publish(new PubSubMessage
                {
                    Event = "task:onexception",
                    Channel = "panteon",
                    Payload = new
                    {
                        TaskName = Name
                    }
                });

                Console.WriteLine(result.Body);
            }
            catch (Exception exception)
            {
                WorkerLogger.Error($"An error occurred while informing about [{Name}] exception. ", exception);
            }
        }
    }
}