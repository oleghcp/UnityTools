using System;
using UnityEngine;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DrawBitMaskAttribute : PropertyAttribute
    {
        internal Type EnumType { get; }

        public DrawBitMaskAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}
