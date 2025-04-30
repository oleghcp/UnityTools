using System;
using OlegHcp.CSharp;
using OlegHcp.NodeBased.Service;

namespace OlegHcpEditor.NodeBased
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomNodeDrawerAttribute : Attribute
    {
        internal Type NodeType { get; }

        public CustomNodeDrawerAttribute(Type nodeType)
        {
            if (!nodeType.IsAssignableTo(typeof(RawNode)))
                throw new ArgumentException($"{nodeType.Name} is not a node type.");

            NodeType = nodeType;
        }
    }
}
