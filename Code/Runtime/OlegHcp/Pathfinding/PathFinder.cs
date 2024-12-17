using System.Collections.Generic;

namespace OlegHcp.Pathfinding
{
    public class PathFinder
    {
        private OrderedStack<IPathNode, float> _frontBuffer = new OrderedStack<IPathNode    , float>();
        private HashSet<IPathNode> _checkBuffer = new HashSet<IPathNode>();
        private NodeDataCollection _nodes = new NodeDataCollection();

        public bool Find(IPathNode origin, IPathNode target, List<IPathNode> result, bool directOrder = true)
        {
            result.Clear();

            if (origin == target)
                return true;

            _frontBuffer.Clear();
            _checkBuffer.Clear();
            _nodes.Clear();

            _frontBuffer.Push(origin, _nodes.GetPassCost(origin));

            while (_frontBuffer.Count > 0)
            {
                IPathNode current = _frontBuffer.Pop();
                _checkBuffer.Add(current);

                IReadOnlyList<PathTransition> transitions = current.GetTransitions();
                for (int i = 0; i < transitions.Count; i++)
                {
                    var (neighbor, cost) = transitions[i];

                    if (_checkBuffer.Contains(neighbor) ||
                        _frontBuffer.Contains(neighbor))
                        continue;

                    _nodes.AddPathData(neighbor, current, cost);

                    if (neighbor == target)
                    {
                        FillResults(origin, target, result, directOrder);
                        return true;
                    }

                    _frontBuffer.Push(neighbor, _nodes.GetPassCost(neighbor));
                }
            }

            return false;
        }

        private void FillResults(IPathNode origin, IPathNode target, List<IPathNode> result, bool directOrder)
        {
            do
            {
                result.Add(target);
                target = _nodes.GetParent(target);
            } while (target != origin);

            if (directOrder)
                result.Reverse();
        }
    }
}
