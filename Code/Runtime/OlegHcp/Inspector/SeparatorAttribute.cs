using System;
using OlegHcp.Mathematics;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SeparatorAttribute : PropertyAttribute
    {
        const float DEFAULT_HEIGHT = 2f;
        const ColorCode DEFAULT_COLOR = ColorCode.Grey;

        internal Color Color { get; }
        internal float Height { get; }

        public SeparatorAttribute(float height, ColorCode color)
        {
            Color = color.ToColor();
            Height = height.ClampMin(0f);
        }

        public SeparatorAttribute(float height) : this(height, DEFAULT_COLOR)
        {

        }

        public SeparatorAttribute(ColorCode color) : this(DEFAULT_HEIGHT, color)
        {

        }

        public SeparatorAttribute() : this(DEFAULT_HEIGHT, DEFAULT_COLOR)
        {
            
        }
    }
}
