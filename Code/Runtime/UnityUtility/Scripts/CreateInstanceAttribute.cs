using System;

namespace UU.Scripts
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class CreateInstanceAttribute : Attribute
    {
        public abstract object Create();
    }
}
