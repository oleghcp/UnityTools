using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class IdentifierAttribute : PropertyAttribute
    {
        internal bool Editable { get; }

        public IdentifierAttribute(bool editable = false)
        {
            Editable = editable;
        }
    }
}
