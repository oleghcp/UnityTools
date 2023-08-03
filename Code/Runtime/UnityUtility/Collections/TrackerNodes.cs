using System;

namespace UnityUtility.Collections
{
    #region Publics
    public interface ITrackerNode
    {
        bool Changed { get; }
        void Check();
        void ForceInvoke();
    }

    public interface ITrackerNode<T> : ITrackerNode
    {
        T Value { get; }
    }
    #endregion

    #region Internals
    internal abstract class BaseNode<T> : ITrackerNode<T>
    {
        private Func<T> _getter;
        private Action _onChangedCallback1;
        private Action<T> _onChangedCallback2;
        private T _cache;
        private bool _changed;

        public bool Changed => _changed;
        public T Value => _cache;

        public BaseNode(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2)
        {
            _getter = getter;
            _cache = getter.Invoke();
            _onChangedCallback1 = onChangedCallback1;
            _onChangedCallback2 = onChangedCallback2;
        }

        public void Check()
        {
            T tmp = _getter();

            if (_changed = !Equal(_cache, tmp))
            {
                _cache = tmp;
                _onChangedCallback1?.Invoke();
                _onChangedCallback2?.Invoke(_cache);
            }
        }

        public void ForceInvoke()
        {
            _onChangedCallback1?.Invoke();
            _onChangedCallback2?.Invoke(_cache);
        }

        protected abstract bool Equal(T a, T b);
    }

    internal class NodeForValueType<T> : BaseNode<T> where T : struct, IEquatable<T>
    {
        public NodeForValueType(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2)
            : base(getter, onChangedCallback1, onChangedCallback2)
        { }

        protected override bool Equal(T a, T b)
        {
            return a.Equals(b);
        }
    }

    internal class NodeForRefType<T> : BaseNode<T> where T : class
    {
        public NodeForRefType(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2)
            : base(getter, onChangedCallback1, onChangedCallback2)
        { }

        protected override bool Equal(T a, T b)
        {
            return a == b;
        }
    }

    internal abstract class BaseDependentNode : ITrackerNode
    {
        private Action _onChangedCallback;

        public abstract bool Changed { get; }

        public BaseDependentNode(Action onChangedCallback)
        {
            _onChangedCallback = onChangedCallback;
        }

        public void Check()
        {
            if (Changed)
                _onChangedCallback();
        }

        public void ForceInvoke()
        {
            _onChangedCallback();
        }
    }

    internal class PrevDependentNode : BaseDependentNode
    {
        private ITrackerNode _previousNode;

        public override bool Changed => _previousNode.Changed;

        public PrevDependentNode(Action onChangedCallback, ITrackerNode previousNode) : base(onChangedCallback)
        {
            _previousNode = previousNode;
        }
    }

    internal class MassDependentNode : BaseDependentNode
    {
        private ITrackerNode[] _dependencies;

        public override bool Changed
        {
            get
            {
                for (int i = 0; i < _dependencies.Length; i++)
                {
                    if (_dependencies[i].Changed)
                        return true;
                }

                return false;
            }
        }

        public MassDependentNode(Action onChangedCallback, ITrackerNode[] dependencies) : base(onChangedCallback)
        {
            _dependencies = dependencies;
        }
    }

    internal class DependentNodeWithValue<T> : ITrackerNode<T>
    {
        private Action<T> _onChangedCallback;
        private ITrackerNode<T> _previousNode;

        public bool Changed => _previousNode.Changed;
        public T Value => _previousNode.Value;

        public DependentNodeWithValue(Action<T> onChangedCallback, ITrackerNode previousNode)
        {
            if (previousNode is ITrackerNode<T> valueNode)
            {
                _onChangedCallback = onChangedCallback;
                _previousNode = valueNode;
            }
            else
            {
                throw new InvalidOperationException($"Previous node does not cache value or value is not {typeof(T)}.");
            }
        }

        public void Check()
        {
            if (Changed)
                _onChangedCallback(_previousNode.Value);
        }

        public void ForceInvoke()
        {
            _onChangedCallback(_previousNode.Value);
        }
    }
    #endregion
}
