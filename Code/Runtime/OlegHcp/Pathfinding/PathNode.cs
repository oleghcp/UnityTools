using System;
using System.Collections.Generic;

namespace OlegHcp.Pathfinding
{
#if UNITY
    [Serializable]
#endif
    public abstract class PathNode : IEquatable<PathNode>
    {
        private int _id;

        [NonSerialized]
        internal PathNode Parent;
        [NonSerialized]
        internal float PassCost;

        public int Id => _id;

        protected PathNode()
        {
            _id = GetHashCode();
        }

        public PathNode(int id)
        {
            _id = id;
        }

        public abstract IReadOnlyList<PathTransition> GetTransitions();

        internal void SetParent(PathNode parent, float passCost)
        {
            Parent = parent;
            PassCost = parent.PassCost + passCost;
        }

        public bool Equals(PathNode other)
        {
            return ReferenceEquals(other, this);
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
