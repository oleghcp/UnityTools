using System;

namespace UnityUtility.SingleScripts
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class CreateInstanceAttribute : Attribute
    {
        public abstract object Create();
    }
}
