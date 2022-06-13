using System;
using UnityEngine;

namespace UnityUtility.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InitListAttribute : PropertyAttribute
    {
        internal Type EnumType { get; }

        public InitListAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class BindSubclassAttribute : Attribute
    {
        internal Type ClassType { get; }

        public BindSubclassAttribute(Type classType)
        {
            ClassType = classType;
        }
    }
}
