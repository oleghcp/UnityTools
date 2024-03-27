using System;
using System.Runtime.Serialization;

namespace OlegHcp.Events
{
    public class OwnerRegisteringException : Exception
    {
        public OwnerRegisteringException() : base() { }
        public OwnerRegisteringException(string message) : base(message) { }
        public OwnerRegisteringException(string message, Exception innerException) : base(message, innerException) { }
        public OwnerRegisteringException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
