using System;
using System.Runtime.Serialization;

namespace OlegHcp.SingleScripts
{
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException() : base() { }
        public ObjectNotFoundException(string message) : base(message) { }
        public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        public ObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
