using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ClampCurveAttribute : PropertyAttribute
    {
        internal Rect Bounds { get; }
        internal Color Color { get; }

        public ClampCurveAttribute(float xMin, float yMin, float xMax, float yMax)
        {
            Bounds = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
            Color = Colours.Lime;
        }

        public ClampCurveAttribute(float xMin, float yMin, float xMax, float yMax, ColorCode color)
        {
            Bounds = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
            Color = color.ToColor();
        }
    }
}
