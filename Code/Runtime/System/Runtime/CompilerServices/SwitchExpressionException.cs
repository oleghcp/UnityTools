#if !UNITY_2021_2_OR_NEWER
namespace System.Runtime.CompilerServices
{
    public sealed class SwitchExpressionException : InvalidOperationException
    {
        private object _unmatchedValue;

        public object UnmatchedValue => _unmatchedValue;

        public override string Message
        {
            get
            {
                if (_unmatchedValue is null)
                    return base.Message;

                return base.Message + Environment.NewLine + GetValueMessage(_unmatchedValue);
            }
        }

        public SwitchExpressionException() : base(GetMessage()) { }
        public SwitchExpressionException(string message) : base(message) { }
        public SwitchExpressionException(string message, Exception innerException) : base(message, innerException) { }
        public SwitchExpressionException(Exception innerException) : base(GetMessage(), innerException) { }

        public SwitchExpressionException(object unmatchedValue) : this()
        {
            _unmatchedValue = unmatchedValue;
        }

        private static string GetMessage()
        {
            return "Non-exhaustive switch expression failed to match its input.";
        }

        private static string GetValueMessage(object value)
        {
            return $"Unmatched value was {value}.";
        }
    }
}
#endif
