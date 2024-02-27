namespace OlegHcp
{
    public sealed class UnsupportedValueException : System.Exception
    {
        public UnsupportedValueException(object value) : base($"Unsupported value for {value.GetType()}: {value}") { }
    }
}
