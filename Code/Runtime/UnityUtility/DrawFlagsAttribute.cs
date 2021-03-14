using System;
using UnityEngine;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DrawFlagsAttribute : PropertyAttribute
    {
        internal Type EnumType { get; }

        public DrawFlagsAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}
