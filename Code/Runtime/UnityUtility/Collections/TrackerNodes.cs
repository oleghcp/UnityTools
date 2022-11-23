using System;

namespace UnityUtility.Collections
{
    #region Publics
    public interface ITrackerNode
    {
        bool Changed { get; }
        void Check();
        void Force();
        void Cache();
    }

    public interface ITrackerNode<T> : ITrackerNode
    {
        T Value { get; }
    }
    #endregion

    #region Internals
    internal class NodeForValueType<T> : ITrackerNode<T> where T : struct, IEquatable<T>
    {
        private Func<T> _getter;
        private Action _onChangedCallback1;
        private Action<T> _onChangedCallback2;
        private T _cache;
        private bool _changed;

        public bool Changed => _changed;
        public T Value => _cache;

        public NodeForValueType(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2)
        {
            _getter = getter;
            _onChangedCallback1 = onChangedCallback1;
            _onChangedCallback2 = onChangedCallback2;
        }

        public void Check()
        {
            T tmp = _getter();

            if (_changed = !_cache.Equals(tmp))
            {
                _cache = tmp;
                _onChangedCallback1?.Invoke();
                _onChangedCallback2?.Invoke(_cache);
            }
        }

        public void Force()
        {
            _cache = _getter();
            _onChangedCallback1?.Invoke();
            _onChangedCallback2?.Invoke(_cache);
        }

        public void Cache()
        {
            _cache = _getter();
        }
    }

    internal class NodeForRefType<T> : ITrackerNode<T> where T : class
    {
        private Func<T> _getter;
        private Action _onChangedCallback1;
        private Action<T> _onChangedCallback2;
        private T _cachedObject;
        private bool _changed;

        public bool Changed => _changed;
        public T Value => _cachedObject;

        public NodeForRefType(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2)
        {
            _getter = getter;
            _onChangedCallback1 = onChangedCallback1;
            _onChangedCallback2 = onChangedCallback2;
        }

        public void Check()
        {
            T tmp = _getter();

            if (_changed = _cachedObject != tmp)
            {
                _cachedObject = tmp;
                _onChangedCallback1?.Invoke();
                _onChangedCallback2?.Invoke(_cachedObject);
            }
        }

        public void Force()
        {
            _cachedObject = _getter();
            _onChangedCallback1?.Invoke();
            _onChangedCallback2?.Invoke(_cachedObject);
        }

        public void Cache()
        {
            _cachedObject = _getter();
        }
    }

    internal class DependentNode : ITrackerNode
    {
        private Action _onChangedCallback;
        private ITrackerNode _previousNode;
        private ITrackerNode[] _dependencies;

        public bool Changed
        {
            get
            {
                if (_previousNode != null)
                    return _previousNode.Changed;

                for (int i = 0; i < _dependencies.Length; i++)
                {
                    if (_dependencies[i].Changed)
                        return true;
                }

                return false;
            }
        }

        public DependentNode(Action onChangedCallback, ITrackerNode previousNode, ITrackerNode[] dependencies)
        {
            _onChangedCallback = onChangedCallback;
            _previousNode = previousNode;
            _dependencies = dependencies;
        }

        public void Check()
        {
            if (Changed)
                _onChangedCallback();
        }

        public void Force()
        {
            _onChangedCallback();
        }

        public void Cache()
        {

        }
    }

    internal class DependentNodeWithValue<T> : ITrackerNode<T>
    {
        private Action<T> _onChangedCallback;
        private ITrackerNode _previousNode;
        private ITrackerNode<T> _valueNode;

        public bool Changed => _previousNode.Changed;
        public T Value => _valueNode.Value;

        public DependentNodeWithValue(Action<T> onChangedCallback, ITrackerNode previousNode)
        {
            if (previousNode is ITrackerNode<T> valueNode)
                _valueNode = valueNode;
            else
                throw new InvalidOperationException($"Previous node does not cache value or value is not {typeof(T)}.");

            _onChangedCallback = onChangedCallback;
            _previousNode = previousNode;
        }

        public void Check()
        {
            if (Changed)
                _onChangedCallback(_valueNode.Value);
        }

        public void Force()
        {
            _onChangedCallback(_valueNode.Value);
        }

        public void Cache()
        {

        }
    }
    #endregion
}
