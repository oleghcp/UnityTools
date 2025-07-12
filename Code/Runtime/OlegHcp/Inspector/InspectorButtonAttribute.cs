using System;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InspectorButtonAttribute : Attribute
    {
        internal string ButtonName;
        internal float Size = 20f;

        public InspectorButtonAttribute(string text = null)
        {
            ButtonName = text;
        }

        public InspectorButtonAttribute(float size, string text = null) : this(text)
        {
            Size = size;
        }
    }
}
