using System;
using UnityEngine;

namespace UnityUtility.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InitListAttribute : PropertyAttribute
    {
        internal readonly Type EnumType;

        public InitListAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BindSubclassAttribute : Attribute
    {
        internal Type ClassType { get; }

        public BindSubclassAttribute(Type classType)
        {
            ClassType = classType;
        }
    }
}
