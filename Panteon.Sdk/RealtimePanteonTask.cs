using System;
using Autofac.Extras.NLog;
using Panteon.Sdk.Configuration;
using Panteon.Sdk.Events;
using Panteon.Sdk.Models;
using Panteon.Sdk.Realtime;

namespace Panteon.Sdk
{
    public abstract class PanteonRealtimeTask : PanteonTask
    {
        protected IPubSubClient PubSubClient { get; private set; }

        protected PanteonRealtimeTask(ILogger taskLogger, ITaskSettings taskSettings, IPubSubClient pubSubClient) : base(taskLogger, taskSettings)
        {
            PubSubClient = pubSubClient;

            OnStarted += Task_Started;
            OnPaused += Task_Paused;
            OnStopped += Task_Stopped;
            OnException += Task_Exception;
            OnEnter += Task_OnEnter;
            OnExit += Task_OnExit;
        }

        private void Task_Started(object sender, TaskStartedEventArgs e)
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
                TaskLogger.Error($"An error occurred while informing about [{Name}] starting operation. ", exception);
            }
        }


        private void Task_Paused(object sender, TaskPausedEventArgs taskPausedEventArgs)
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
                TaskLogger.Error($"An error occurred while informing about [{Name}] pausing operation. ", exception);
            }
        }

        private void Task_Stopped(object sender, TaskStoppedEventArgs taskStoppedEventArgs)
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
                TaskLogger.Error($"An error occurred while informing about [{Name}] stopping operation. ", exception);
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

        private void Task_OnEnter(object sender, TaskEventArgs e)
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
                TaskLogger.Error($"An error occurred while informing about [{Name}] action enter. ", exception);
            }
        }

        private void Task_OnExit(object sender, TaskEventArgs e)
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
                TaskLogger.Error($"An error occurred while informing about [{Name}] action exit. ", exception);
            }
        }


        private void Task_Exception(object sender, TaskExceptionEventArgs e)
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
                TaskLogger.Error($"An error occurred while informing about [{Name}] exception. ", exception);
            }
        }
    }
}