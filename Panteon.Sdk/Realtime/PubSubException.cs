using System;
using System.Runtime.Serialization;

namespace Panteon.Sdk.Realtime
{
    [Serializable]
    public class PubSubException : Exception
    {
        public PubSubException()
        {
        }

        public PubSubException(string message) : base(message)
        {
        }

        public PubSubException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PubSubException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}