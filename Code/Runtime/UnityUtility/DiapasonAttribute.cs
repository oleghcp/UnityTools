using System;
using UnityEngine;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DiapasonAttribute : PropertyAttribute
    {
        internal float MinValue { get; }

        public DiapasonAttribute() { }

        public DiapasonAttribute(float minValue)
        {
            MinValue = minValue;
        }
    }
}
