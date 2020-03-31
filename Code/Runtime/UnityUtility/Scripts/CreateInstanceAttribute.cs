using System;

namespace UnityUtility.Scripts
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class CreateInstanceAttribute : Attribute
    {
        public abstract object Create();
    }
}
