using System;
using OlegHcp.CSharp;
using OlegHcp.NodeBased;
using OlegHcp.NodeBased.Service;

namespace OlegHcpEditor.NodeBased
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomGraphPanelDrawerAttribute : Attribute
    {
        internal Type GraphType { get; }

        public CustomGraphPanelDrawerAttribute(Type graphType)
        {
            if (!graphType.IsAssignableTo(typeof(RawGraph)))
                throw new ArgumentException($"{graphType.Name} is not a graph type.");

            GraphType = graphType;
        }
    }
}
