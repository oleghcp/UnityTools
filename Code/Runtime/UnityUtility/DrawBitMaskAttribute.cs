using System;
using UnityEngine;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DrawBitMaskAttribute : PropertyAttribute
    {
        internal readonly Type EnumType;

        public DrawBitMaskAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}
