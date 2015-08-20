namespace Panteon.Sdk.Realtime
{
    public interface IPubSubClient
    {
        IPubSubResult Publish<T>(T message) where T : class, IPubSubMessage;
    }
}