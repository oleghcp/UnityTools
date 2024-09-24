using System;

namespace OlegHcp
{
    public class UnsupportedValueException : InvalidOperationException
    {
        public UnsupportedValueException(object value) : base(GetMessage(value)) { }

        private static string GetMessage(object value)
        {
            return $"Unsupported value for {value.GetType()}: {value}";
        }
    }
}
