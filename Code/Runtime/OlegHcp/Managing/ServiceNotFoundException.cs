using System;
using System.Runtime.Serialization;

namespace OlegHcp.Managing
{
    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException() : base() { }
        public ServiceNotFoundException(string message) : base(message) { }
        public ServiceNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        public ServiceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
