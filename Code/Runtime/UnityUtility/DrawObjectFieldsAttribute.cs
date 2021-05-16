using System;
using UnityEngine;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DrawObjectFieldsAttribute : PropertyAttribute
    {
        internal bool NeedIndent { get; }

        public DrawObjectFieldsAttribute(bool needIndent = true)
        {
            NeedIndent = needIndent;
        }
    }
}
