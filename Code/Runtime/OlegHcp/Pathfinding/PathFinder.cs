using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Pathfinding
{
    [Serializable]
    public class PathFinder
    {
        private OrderedStack<PathNode, float> _frontBuffer = new OrderedStack<PathNode, float>();
        private HashSet<int> _acceptableBuffer = new HashSet<int>();

        public void FindPath(PathNode startNode, PathNode endNode, List<PathNode> result)
        {
            _frontBuffer.Push(startNode, startNode.PassCost);
            while (_frontBuffer.Count > 0 && !_acceptableBuffer.Contains(endNode.Id))
            {
                PathNode current = _frontBuffer.Pop();
                _acceptableBuffer.Add(current.Id);

                IReadOnlyList<PathTransition> transitions = current.GetTransitions();
                for (int i = 0; i < transitions.Count; i++)
                {
                    var (neighbor, cost) = transitions[i];

                    if (_acceptableBuffer.Contains(neighbor.Id) || _frontBuffer.Contains(neighbor))
                        continue;

                    neighbor.SetParent(current, cost);
                    _frontBuffer.Push(neighbor, neighbor.PassCost);
                }
            }

            result.Clear();

            if (_acceptableBuffer.Contains(endNode.Id))
            {
                int count = endNode.PathNumber;
                result.SetCount(count);

                for (int i = count - 1; i >= 0; i--)
                {
                    result[i] = endNode;
                    endNode = endNode.Parent;
                }
            }

            _frontBuffer.Clear();
            _acceptableBuffer.Clear();
        }
    }
}
