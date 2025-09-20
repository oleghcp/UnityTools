using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TypeNameAttribute : PropertyAttribute
    {
        internal readonly Type TargetType;
        internal readonly bool SkipAbstract;

        public TypeNameAttribute(Type baseType, bool skipAbstract = false)
        {
            TargetType = baseType;
            SkipAbstract = skipAbstract;
        }
    }
}
