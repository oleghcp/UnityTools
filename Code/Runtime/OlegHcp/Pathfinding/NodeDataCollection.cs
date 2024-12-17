using System.Collections.Generic;

namespace OlegHcp.Pathfinding
{
    internal class NodeDataCollection : Dictionary<IPathNode, NodeDataCollection.NodeData>
    {
        public void AddPathData(IPathNode node, IPathNode parent, float passCost)
        {
            base[node] = new NodeData()
            {
                Parent = parent,
                PassCost = GetPassCost(parent) + passCost,
            };
        }

        public float GetPassCost(IPathNode node)
        {
            if (TryGetValue(node, out NodeData value))
                return value.PassCost;

            return default;
        }

        public IPathNode GetParent(IPathNode node)
        {
            if (TryGetValue(node, out NodeData value))
                return value.Parent;

            return default;
        }

        public struct NodeData
        {
            public IPathNode Parent { get; set; }
            public float PassCost { get; set; }
        }
    }
}
