using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyLabelAttribute : PropertyAttribute
    {
        internal readonly string Label;

        public PropertyLabelAttribute(string label)
        {
            Label = label;
        }
    }
}
