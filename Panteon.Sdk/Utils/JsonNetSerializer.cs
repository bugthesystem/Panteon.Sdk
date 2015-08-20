using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Panteon.Sdk.Utils
{
    public class JsonNetSerializer : IJsonSerializer
    {
        public  string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, new KeyValuePairConverter());
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}