using System;
using UnityEngine;

namespace UnityUtility.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DiapasonAttribute : PropertyAttribute
    {
        internal float MinValue { get; }
        internal float MaxValue { get; }

        public DiapasonAttribute()
        {
            MinValue = float.NegativeInfinity;
            MaxValue = float.PositiveInfinity;
        }

        public DiapasonAttribute(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
