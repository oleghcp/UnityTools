using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LayerFieldAttribute : PropertyAttribute
    {
        internal bool AsMask { get; }

        public LayerFieldAttribute(bool asMask = false)
        {
            AsMask = asMask;
        }
    }
}
