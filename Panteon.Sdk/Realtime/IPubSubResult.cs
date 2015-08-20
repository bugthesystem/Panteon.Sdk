using System.Net;

namespace Panteon.Sdk.Realtime
{
    public interface IPubSubResult
    {
        string Body { get; set; }
        HttpStatusCode StatusCode { get; set; }
    }

    public class PubSubResult : IPubSubResult
    {
        public string Body { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}