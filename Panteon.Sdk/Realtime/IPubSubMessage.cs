namespace Panteon.Sdk.Realtime
{
    public interface IPubSubMessage
    {
        string Channel { get; set; }
        string Event { get; set; }
        object Payload { get; set; }
    }
}