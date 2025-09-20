using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LabelAttribute : PropertyAttribute
    {
        internal readonly string Label;

        public LabelAttribute(string label)
        {
            Label = label;
        }
    }
}
