using System;
using Autofac.Extras.NLog;
using Panteon.Sdk.Configuration;
using Panteon.Sdk.Models;
using Panteon.Sdk.Realtime;

namespace Panteon.Sdk
{
    public abstract class PanteonRealtimeTask : PanteonTask
    {
        protected IPubSubClient PubSubClient { get; private set; }

        protected PanteonRealtimeTask(ILogger logger, ITaskSettings taskSettings, IPubSubClient pubSubClient) : base(logger, taskSettings)
        {
            PubSubClient = pubSubClient;
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
    }
}