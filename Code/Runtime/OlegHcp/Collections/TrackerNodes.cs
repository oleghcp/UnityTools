using System;

namespace OlegHcp.Collections
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
        T PrevValue { get; }
        T CurValue { get; }
    }
    #endregion

    #region Internals
    internal abstract class BaseNode<T> : ITrackerNode<T>
    {
        private Func<T> _getter;
        private Action _onChangedCallback1;
        private Action<T> _onChangedCallback2;
        private Action<T, T> _onChangedCallback3;
        private T _prevValue;
        private T _curValue;
        private bool _changed;

        public bool Changed => _changed;
        public T PrevValue => _prevValue;
        public T CurValue => _curValue;

        public BaseNode(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2, Action<T, T> onChangedCallback3)
        {
            _getter = getter;
            _curValue = getter.Invoke();
            _onChangedCallback1 = onChangedCallback1;
            _onChangedCallback2 = onChangedCallback2;
            _onChangedCallback3 = onChangedCallback3;
        }

        public void Check()
        {
            T newValue = _getter();

            if (_changed = !Equal(_curValue, newValue))
            {
                try
                {
                    _onChangedCallback1?.Invoke();
                    _onChangedCallback2?.Invoke(newValue);
                    _onChangedCallback3?.Invoke(_curValue, newValue);
                }
                finally
                {
                    _prevValue = _curValue;
                    _curValue = newValue;
                }
            }
        }

        public void ForceInvoke()
        {
            _onChangedCallback1?.Invoke();
            _onChangedCallback2?.Invoke(_curValue);
            _onChangedCallback3?.Invoke(_prevValue, _curValue);
        }

        protected abstract bool Equal(T a, T b);
    }

    internal class NodeForValueType<T> : BaseNode<T> where T : struct, IEquatable<T>
    {
        public NodeForValueType(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2, Action<T, T> onChangedCallback3)
            : base(getter, onChangedCallback1, onChangedCallback2, onChangedCallback3)
        { }

        protected override bool Equal(T a, T b)
        {
            return a.Equals(b);
        }
    }

    internal class NodeForRefType<T> : BaseNode<T> where T : class
    {
        public NodeForRefType(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2, Action<T, T> onChangedCallback3)
            : base(getter, onChangedCallback1, onChangedCallback2, onChangedCallback3)
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

    internal class PrevDependentNodeWithValue<T> : ITrackerNode<T>
    {
        private Action<T> _onChangedCallback1;
        private Action<T, T> _onChangedCallback2;
        private ITrackerNode<T> _previousNode;

        public bool Changed => _previousNode.Changed;
        public T PrevValue => _previousNode.PrevValue;
        public T CurValue => _previousNode.CurValue;

        public PrevDependentNodeWithValue(Action<T> onChangedCallback1, Action<T, T> onChangedCallback2, ITrackerNode previousNode)
        {
            if (previousNode is ITrackerNode<T> valueNode)
            {
                _onChangedCallback1 = onChangedCallback1;
                _onChangedCallback2 = onChangedCallback2;
                _previousNode = valueNode;
            }
            else
            {
                throw new InvalidOperationException($"Previous node does not cache value or value is not {typeof(T)}.");
            }
        }

        public void Check()
        {
            if (_previousNode.Changed)
            {
                _onChangedCallback1?.Invoke(_previousNode.CurValue);
                _onChangedCallback2?.Invoke(_previousNode.PrevValue, _previousNode.CurValue);
            }
        }

        public void ForceInvoke()
        {
            _onChangedCallback1?.Invoke(_previousNode.CurValue);
            _onChangedCallback2?.Invoke(_previousNode.PrevValue, _previousNode.CurValue);
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
    #endregion
}
