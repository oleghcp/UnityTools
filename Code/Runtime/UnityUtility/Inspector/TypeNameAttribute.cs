using System;
using UnityEngine;

namespace UnityUtility.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TypeNameAttribute : PropertyAttribute
    {
        internal Type TargetType { get; }

        public TypeNameAttribute(Type baseType)
        {
            TargetType = baseType;
        }
    }
}
