using System.Collections.Generic;

namespace OlegHcp.Pathfinding
{
    public class PathFinder
    {
        private OrderedStack<PathNode, float> _frontBuffer = new OrderedStack<PathNode, float>();
        private HashSet<PathNode> _checkBuffer = new HashSet<PathNode>();

        public bool Find(PathNode origin, PathNode target, List<PathNode> result, bool directOrder = true)
        {
            result.Clear();

            if (origin == target)
                return true;

            _frontBuffer.Clear();
            _checkBuffer.Clear();

            _frontBuffer.Push(origin, origin.PassCost);

            while (_frontBuffer.Count > 0)
            {
                PathNode current = _frontBuffer.Pop();
                _checkBuffer.Add(current);

                IReadOnlyList<PathTransition> transitions = current.GetTransitions();
                for (int i = 0; i < transitions.Count; i++)
                {
                    var (neighbor, cost) = transitions[i];

                    if (_checkBuffer.Contains(neighbor) ||
                        _frontBuffer.Contains(neighbor))
                        continue;

                    neighbor.AddPathData(current, cost);

                    if (neighbor == target)
                    {
                        FillResults(origin, target, result, directOrder);
                        return true;
                    }

                    _frontBuffer.Push(neighbor, neighbor.PassCost);
                }
            }

            return false;
        }

        private static void FillResults(PathNode origin, PathNode target, List<PathNode> result, bool directOrder)
        {
            do
            {
                result.Add(target);
                target = target.Parent;
            } while (target != origin);

            if (directOrder)
                result.Reverse();
        }
    }
}
