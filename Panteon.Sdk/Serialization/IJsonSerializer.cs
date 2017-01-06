namespace Panteon.Sdk.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);
        T Deserialize<T>(string json);
    }
}