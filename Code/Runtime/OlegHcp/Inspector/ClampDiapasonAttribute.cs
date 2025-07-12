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
        internal int MaxInt => (int)Max.ClampMax(int.MaxValue - 500);
        internal bool Slider { get; }

        public ClampDiapasonAttribute(float min, float max = float.PositiveInfinity)
        {
            if (max < min)
            {
                Debug.LogError("Incorrect min-max order.");
                return;
            }

            Min = min;
            Max = max;
        }

        public ClampDiapasonAttribute(float min, float max, bool slider)
        {
            if (max < min)
            {
                Debug.LogError("Incorrect min-max order.");
                return;
            }

            Min = min;
            Max = max;
            Slider = slider;
        }
    }
}
