using System;
using UnityEngine;

namespace UnityUtility.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DrawObjectFieldsAttribute : PropertyAttribute
    {
        internal bool NeedIndent { get; }

        public DrawObjectFieldsAttribute(bool needIndent = true)
        {
            NeedIndent = needIndent;
        }
    }
}
