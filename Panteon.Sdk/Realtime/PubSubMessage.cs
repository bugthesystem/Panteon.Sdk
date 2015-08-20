namespace Panteon.Sdk.Realtime
{
    public class PubSubMessage : IPubSubMessage
    {
        public string Channel { get; set; }

        public string Event { get; set; }

        public object Payload { get; set; }
    }
}