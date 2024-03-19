﻿using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InspectorButtonAttribute : PropertyAttribute
    {
        internal string ButtonName { get; private set; }
        internal float Size { get; private set; } = 20f;

        public InspectorButtonAttribute(string text = null)
        {
            ButtonName = text;
        }

        public InspectorButtonAttribute(string text, float size) : this(text)
        {
            Size = size;
        }
    }
}
