using System;

namespace OlegHcp.SingleScripts
{
    public class ObjectNotFoundException : SystemException
    {
        private Type _objectType;

        public Type ObjectType => _objectType;

        public override string Message
        {
            get
            {
                if (_objectType is null)
                    return base.Message;

                return base.Message + Environment.NewLine + GetValueMessage(_objectType);
            }
        }

        public ObjectNotFoundException() : base(GetMessage()) { }
        public ObjectNotFoundException(string message) : base(message) { }
        public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        public ObjectNotFoundException(Exception innerException) : base(GetMessage(), innerException) { }

        public ObjectNotFoundException(Type type) : this()
        {
            _objectType = type;
        }

        private static string GetMessage()
        {
            return $"There is no any instance of object.";
        }

        private static string GetValueMessage(Type type)
        {
            return $"Object type is {type.FullName}.";
        }
    }
}
