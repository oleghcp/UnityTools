using System;
using OlegHcp.Mathematics;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ClampDiapasonAttribute : PropertyAttribute
    {
        internal float Min { get; }
        internal float Max { get; }
        internal int MinInt => (int)Min;
        internal int MaxInt { get; }

        public ClampDiapasonAttribute(float min, float max = float.PositiveInfinity)
        {
            Min = min;
            Max = max;

            int maxInt = (int)max;
            MaxInt = maxInt.Sign() == max.Sign() ? maxInt : int.MaxValue;
        }
    }
}
