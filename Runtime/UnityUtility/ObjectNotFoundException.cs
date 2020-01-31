using System;

namespace UU
{
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException() : base() { }
        public ObjectNotFoundException(string message) : base(message) { }
    }
}
