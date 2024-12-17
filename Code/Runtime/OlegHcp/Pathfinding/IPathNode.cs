using System.Collections.Generic;

namespace OlegHcp.Pathfinding
{
    public interface IPathNode
    {
        IReadOnlyList<PathTransition> GetTransitions();
    }

    public struct PathTransition
    {
        public IPathNode Neighbor { get; }
        public float Cost { get; }

        public PathTransition(IPathNode neighbor, float cost)
        {
            Neighbor = neighbor;
            Cost = cost;
        }

        public void Deconstruct(out IPathNode neighbor, out float cost)
        {
            neighbor = Neighbor;
            cost = Cost;
        }
    }
}
