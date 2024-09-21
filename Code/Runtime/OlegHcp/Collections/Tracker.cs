using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using OlegHcp.Tools;

namespace OlegHcp.Collections
{
    public class Tracker
    {
        private List<ITrackerNode> _nodes = new List<ITrackerNode>();

        public void Refresh()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Check();
            }
        }

        public void ForceInvoke()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].ForceInvoke();
            }
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter) where T : struct, IEquatable<T>
        {
            return _nodes.Place(new NodeForValueType<T>(getter, null, null, null));
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter, Action onChangedCallback) where T : struct, IEquatable<T>
        {
            return _nodes.Place(new NodeForValueType<T>(getter, onChangedCallback, null, null));
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter, Action<T> onChangedCallback) where T : struct, IEquatable<T>
        {
            return _nodes.Place(new NodeForValueType<T>(getter, null, onChangedCallback, null));
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter, Action<T, T> onChangedCallback) where T : struct, IEquatable<T>
        {
            return _nodes.Place(new NodeForValueType<T>(getter, null, null, onChangedCallback));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter) where T : class
        {
            return _nodes.Place(new NodeForRefType<T>(getter, null, null, null));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter, Action onChangedCallback) where T : class
        {
            return _nodes.Place(new NodeForRefType<T>(getter, onChangedCallback, null, null));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter, Action<T> onChangedCallback) where T : class
        {
            return _nodes.Place(new NodeForRefType<T>(getter, null, onChangedCallback, null));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter, Action<T, T> onChangedCallback) where T : class
        {
            return _nodes.Place(new NodeForRefType<T>(getter, null, null, onChangedCallback));
        }

        public ITrackerNode AddNodeBasedOnPrev(Action onChangedCallback)
        {
            if (_nodes.Count == 0)
                throw ThrowErrors.EmptyTracker();

            return _nodes.Place(new PrevDependentNode(onChangedCallback, _nodes.FromEnd(0)));
        }

        public ITrackerNode AddNodeBasedOnPrev<T>(Action<T> onChangedCallback)
        {
            if (_nodes.Count == 0)
                throw ThrowErrors.EmptyTracker();

            return _nodes.Place(new PrevDependentNodeWithValue<T>(onChangedCallback, null, _nodes.FromEnd(0)));
        }

        public ITrackerNode AddNodeBasedOnPrev<T>(Action<T, T> onChangedCallback)
        {
            if (_nodes.Count == 0)
                throw ThrowErrors.EmptyTracker();

            return _nodes.Place(new PrevDependentNodeWithValue<T>(null, onChangedCallback, _nodes.FromEnd(0)));
        }

        public ITrackerNode AddDependentNode(Action onChangedCallback, params ITrackerNode[] dependencies)
        {
            return _nodes.Place(new MassDependentNode(onChangedCallback, dependencies));
        }

        public ITrackerNode AddCustomNode(ITrackerNode node)
        {
            return _nodes.Place(node);
        }
    }
}
