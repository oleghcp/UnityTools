using System;

namespace OlegHcp.NodeBased.Service
{
#if UNITY_EDITOR
    internal static class GraphUtility
    {
        internal static NodeType GetNodeType(Type type)
        {
            if (type == typeof(ExitNode))
                return NodeType.Exit;
            else if (type == typeof(HubNode))
                return NodeType.Hub;
            else if (type == typeof(CommonNode))
                return NodeType.Common;
            else
                return NodeType.Regular;
        }
    }
#endif
}
