using System;
using UnityEngine;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeNameAttribute : PropertyAttribute
    {
        internal Type TargetType { get; }

        public TypeNameAttribute(Type baseType)
        {
            TargetType = baseType;
        }
    }
}
