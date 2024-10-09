using System;
using System.Collections.Generic;

namespace OlegHcp.Pathfinding
{
#if UNITY
    [Serializable]
#endif
    public abstract class PathNode
    {
        [NonSerialized]
        internal PathNode Parent;
        [NonSerialized]
        internal float PassCost;

        public abstract IReadOnlyList<PathTransition> GetTransitions();

        internal void AddPathData(PathNode parent, float passCost)
        {
            Parent = parent;
            PassCost = parent.PassCost + passCost;
        }
    }

#if UNITY
    [Serializable]
#endif
    public struct PathTransition
    {
        public PathNode Neighbor { get; }
        public float Cost { get; }

        public PathTransition(PathNode neighbor, float cost)
        {
            Neighbor = neighbor;
            Cost = cost;
        }

        public void Deconstruct(out PathNode neighbor, out float cost)
        {
            neighbor = Neighbor;
            cost = Cost;
        }
    }
}
