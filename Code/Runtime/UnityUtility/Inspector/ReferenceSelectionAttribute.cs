using System;
using UnityEngine;

namespace UnityUtility.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReferenceSelectionAttribute : PropertyAttribute
    {
        internal bool ShortButtonText;

        public ReferenceSelectionAttribute() { }

        public ReferenceSelectionAttribute(bool shortButtonText)
        {
            ShortButtonText = shortButtonText;
        }
    }
}
