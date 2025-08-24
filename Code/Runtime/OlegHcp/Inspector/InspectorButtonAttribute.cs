using System;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InspectorButtonAttribute : Attribute
    {
        internal string ButtonName;
        internal float Size = 20f;
        internal EditorPlayState ShowState;

        public InspectorButtonAttribute(string text = null)
        {
            ButtonName = text;
        }

        public InspectorButtonAttribute(EditorPlayState showState, string text = null)
        {
            ShowState = showState;
            ButtonName = text;
        }

        public InspectorButtonAttribute(float size, string text = null)
        {
            ButtonName = text;
            Size = size;
        }

        public InspectorButtonAttribute(float size, EditorPlayState showState, string text = null)
        {
            Size = size;
            ShowState = showState;
            ButtonName = text;
        }
    }

    public enum EditorPlayState
    {
        Both = 0,
        PlayModeActive,
        PlayModeStopped,
    }
}
