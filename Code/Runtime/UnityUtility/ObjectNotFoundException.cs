using System;

namespace UnityUtility
{
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException() : base() { }
        public ObjectNotFoundException(string message) : base(message) { }
    }
}
