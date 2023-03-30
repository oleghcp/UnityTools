using System;
using UnityUtility.CSharp;
using UnityUtility.NodeBased.Service;

namespace UnityUtilityEditor.Window.NodeBased
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CustomNodeDrawerAttribute : Attribute
    {
        internal Type NodeType { get; }

        public CustomNodeDrawerAttribute(Type nodeType)
        {
            if (!nodeType.IsAssignableTo(typeof(RawNode)))
                throw new ArgumentException($"Given type is not assignable to {typeof(RawNode).FullName}");

            NodeType = nodeType;
        }
    }
}
