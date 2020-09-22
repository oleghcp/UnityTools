using System;
using UnityEngine;

namespace UnityUtility.Code.Runtime.UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeNameAttribute : PropertyAttribute
    {
        public Type TargetType { get; }

        public TypeNameAttribute(Type baseType)
        {
            TargetType = baseType;
        }
    }
}
