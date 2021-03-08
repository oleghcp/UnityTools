using System;
using System.Collections.Generic;
using UnityUtilityTools;
using static UnityUtility.Collections.TrackerNodes;

namespace UnityUtility.Collections
{
    public class Tracker : IRefreshable
    {
        private List<IActiveNode> _nodes = new List<IActiveNode>();

        public void Refresh()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Check();
            }
        }

        public void Force()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Force();
            }
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter) where T : struct, IEquatable<T>
        {
            return _nodes.Place(new NodeForValueType<T>(getter, null, null));
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter, Action onChangedCallback) where T : struct, IEquatable<T>
        {
            return _nodes.Place(new NodeForValueType<T>(getter, onChangedCallback, null));
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter, Action<T> onChangedCallback) where T : struct, IEquatable<T>
        {
            return _nodes.Place(new NodeForValueType<T>(getter, null, onChangedCallback));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter) where T : class
        {
            return _nodes.Place(new NodeForRefType<T>(getter, null, null));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter, Action onChangedCallback) where T : class
        {
            return _nodes.Place(new NodeForRefType<T>(getter, onChangedCallback, null));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter, Action<T> onChangedCallback) where T : class
        {
            return _nodes.Place(new NodeForRefType<T>(getter, null, onChangedCallback));
        }

        public ITrackerNode AddNodeBasedOnPrev(Action onChangedCallback)
        {
            if (_nodes.Count == 0)
                throw Errors.EmptyTraker();

            return _nodes.Place(new DependentNode(onChangedCallback, _nodes.FromEnd(0), null));
        }

        public ITrackerNode AddNodeBasedOnPrev<T>(Action<T> onChangedCallback)
        {
            if (_nodes.Count == 0)
                throw Errors.EmptyTraker();

            return _nodes.Place(new DependentNodeWithValue<T>(onChangedCallback, _nodes.FromEnd(0)));
        }

        public ITrackerNode AddDependentNode(Action onChangedCallback, params ITrackerNode[] dependencies)
        {
            return _nodes.Place(new DependentNode(onChangedCallback, null, dependencies));
        }

        public ITrackerNode AddCustomNode(CustomTrackerNode node)
        {
            return _nodes.Place(new CustomNodeWrapper(node));
        }

        public ITrackerNode AddCustomNode<T>(CustomTrackerNode<T> node)
        {
            return _nodes.Place(new CustomNodeWrapper<T>(node));
        }
    }
}
