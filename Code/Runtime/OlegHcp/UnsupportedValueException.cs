using System;
using System.Runtime.Serialization;

namespace OlegHcp
{
    public sealed class UnsupportedValueException : Exception
    {
        public UnsupportedValueException() : base() { }
        public UnsupportedValueException(object value) : base(GetMessage(value)) { }
        public UnsupportedValueException(object value, Exception innerException) : base(GetMessage(value), innerException) { }
        public UnsupportedValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private static string GetMessage(object value)
        {
            return $"Unsupported value for {value.GetType()}: {value}";
        }
    }
}
