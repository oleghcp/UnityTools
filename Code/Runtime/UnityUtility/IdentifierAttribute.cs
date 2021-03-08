using System;
using UnityEngine;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IdentifierAttribute : PropertyAttribute
    {
        internal bool Editable { get; }

        public IdentifierAttribute(bool editable = false)
        {
            Editable = editable;
        }
    }
}
