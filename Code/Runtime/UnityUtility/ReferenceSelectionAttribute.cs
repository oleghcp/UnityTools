using System;
using UnityEngine;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReferenceSelectionAttribute : PropertyAttribute
    {
        internal bool PrettyButton;

        public ReferenceSelectionAttribute() { }

        public ReferenceSelectionAttribute(bool prettyButton)
        {
            PrettyButton = prettyButton;
        }
    }
}
