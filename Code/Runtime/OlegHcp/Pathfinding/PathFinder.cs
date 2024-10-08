﻿using System;
using System.Collections.Generic;

namespace OlegHcp.Pathfinding
{
#if UNITY
    [Serializable]
#endif
    public class PathFinder
    {
        private OrderedStack<PathNode, float> _frontBuffer = new OrderedStack<PathNode, float>();
        private HashSet<int> _acceptableBuffer = new HashSet<int>();

        public void Find(PathNode origin, PathNode target, List<PathNode> result, bool directOrder = true)
        {
            result.Clear();
            _frontBuffer.Clear();
            _acceptableBuffer.Clear();

            _frontBuffer.Push(origin, origin.PassCost);
            while (_frontBuffer.Count > 0 && !_acceptableBuffer.Contains(target.Id))
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

            if (_acceptableBuffer.Contains(target.Id))
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
