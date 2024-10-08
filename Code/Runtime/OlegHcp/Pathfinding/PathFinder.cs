using System;
using System.Collections.Generic;

namespace OlegHcp.Pathfinding
{
#if UNITY
    [Serializable]
#endif
    public class PathFinder
    {
        private OrderedStack<PathNode, float> _frontBuffer = new OrderedStack<PathNode, float>();
        private HashSet<PathNode> _checkBuffer = new HashSet<PathNode>();

        public void Find(PathNode origin, PathNode target, List<PathNode> result, bool directOrder = true)
        {
            result.Clear();
            _frontBuffer.Clear();
            _checkBuffer.Clear();

            _frontBuffer.Push(origin, origin.PassCost);
            while (_frontBuffer.Count > 0 && !_checkBuffer.Contains(target))
            {
                PathNode current = _frontBuffer.Pop();
                _checkBuffer.Add(current);

                IReadOnlyList<PathTransition> transitions = current.GetTransitions();
                for (int i = 0; i < transitions.Count; i++)
                {
                    var (neighbor, cost) = transitions[i];

                    if (_checkBuffer.Contains(neighbor) || _frontBuffer.Contains(neighbor))
                        continue;

                    neighbor.SetParent(current, cost);
                    _frontBuffer.Push(neighbor, neighbor.PassCost);
                }
            }

            if (_checkBuffer.Contains(target))
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
}
