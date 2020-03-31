using System;
using UnityEngine;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PercentRangeAttribute : PropertyAttribute
    {
        internal readonly float Min;
        internal readonly float Max;

        public PercentRangeAttribute()
        {
            Max = 100f;
        }

        public PercentRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
