﻿using System;

namespace UnityUtility.Collections
{
    public interface ITrackerNode
    {
        bool Changed { get; }
    }

    public abstract class CustomTrackerNode
    {
        public abstract bool Changed { get; }
        public abstract void Check();
        public abstract void Force();
    }

    public abstract class CustomTrackerNode<T> : CustomTrackerNode
    {
        public abstract T Value { get; }
    }

    internal static class TrackerNodes
    {
        public interface IActiveNode : ITrackerNode
        {
            void Check();
            void Force();
        }

        public interface IValueKeeper<T>
        {
            T Value { get; }
        }

        // -- //

        public class NodeForValueType<T> : IActiveNode, IValueKeeper<T> where T : struct, IEquatable<T>
        {
            private Func<T> m_getter;
            private Action m_onChangedCallback1;
            private Action<T> m_onChangedCallback2;
            private T m_cache;
            private bool m_changed;

            public bool Changed => m_changed;
            public T Value => m_cache;

            public NodeForValueType(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2)
            {
                m_getter = getter;
                m_onChangedCallback1 = onChangedCallback1;
                m_onChangedCallback2 = onChangedCallback2;
            }

            public void Check()
            {
                T tmp = m_getter();

                if (m_changed = !m_cache.Equals(tmp))
                {
                    m_cache = tmp;
                    m_onChangedCallback1?.Invoke();
                    m_onChangedCallback2?.Invoke(m_cache);
                }
            }

            public void Force()
            {
                m_cache = m_getter();
                m_onChangedCallback1?.Invoke();
                m_onChangedCallback2?.Invoke(m_cache);
            }
        }

        // -- //

        public class NodeForRefType<T> : IActiveNode, IValueKeeper<T> where T : class
        {
            private Func<T> m_getter;
            private Action m_onChangedCallback1;
            private Action<T> m_onChangedCallback2;
            private T m_cachedObject;
            private bool m_changed;

            public bool Changed => m_changed;
            public T Value => m_cachedObject;

            public NodeForRefType(Func<T> getter, Action onChangedCallback1, Action<T> onChangedCallback2)
            {
                m_getter = getter;
                m_onChangedCallback1 = onChangedCallback1;
                m_onChangedCallback2 = onChangedCallback2;
            }

            public void Check()
            {
                T tmp = m_getter();

                if (m_changed = m_cachedObject != tmp)
                {
                    m_cachedObject = tmp;
                    m_onChangedCallback1?.Invoke();
                    m_onChangedCallback2?.Invoke(m_cachedObject);
                }
            }

            public void Force()
            {
                m_cachedObject = m_getter();
                m_onChangedCallback1?.Invoke();
                m_onChangedCallback2?.Invoke(m_cachedObject);
            }
        }

        // -- //

        public class DependentNode : IActiveNode
        {
            private Action m_onChangedCallback;
            private ITrackerNode m_previousNode;
            private ITrackerNode[] m_dependencies;

            public bool Changed
            {
                get
                {
                    if (m_previousNode != null)
                        return m_previousNode.Changed;

                    for (int i = 0; i < m_dependencies.Length; i++)
                    {
                        if (m_dependencies[i].Changed)
                            return true;
                    }

                    return false;
                }
            }

            public DependentNode(Action onChangedCallback, ITrackerNode previousNode, ITrackerNode[] dependencies)
            {
                m_onChangedCallback = onChangedCallback;
                m_previousNode = previousNode;
                m_dependencies = dependencies;
            }

            public void Check()
            {
                if (Changed)
                    m_onChangedCallback();
            }

            public void Force()
            {
                m_onChangedCallback();
            }
        }

        public class DependentNodeWithValue<T> : IActiveNode, IValueKeeper<T>
        {
            private Action<T> m_onChangedCallback;
            private ITrackerNode m_previousNode;
            private IValueKeeper<T> m_valueNode;

            public bool Changed => m_previousNode.Changed;
            public T Value => m_valueNode.Value;

            public DependentNodeWithValue(Action<T> onChangedCallback, ITrackerNode previousNode)
            {
                if (previousNode is IValueKeeper<T> valueNode)
                    m_valueNode = valueNode;
                else
                    throw new InvalidOperationException($"Previous node does not cache value or value is not {typeof(T)}.");

                m_onChangedCallback = onChangedCallback;
                m_previousNode = previousNode;
            }

            public void Check()
            {
                if (Changed)
                    m_onChangedCallback(m_valueNode.Value);
            }

            public void Force()
            {
                m_onChangedCallback(m_valueNode.Value);
            }
        }

        // -- //

        public class CustomNodeWrapper : IActiveNode
        {
            protected CustomTrackerNode m_node;

            public bool Changed => m_node.Changed;

            public CustomNodeWrapper(CustomTrackerNode node)
            {
                m_node = node;
            }

            public void Check()
            {
                m_node.Check();
            }

            public void Force()
            {
                m_node.Force();
            }
        }        

        public class CustomNodeWrapper<T> : CustomNodeWrapper, IValueKeeper<T>
        {
            public T Value => (m_node as CustomTrackerNode<T>).Value;

            public CustomNodeWrapper(CustomTrackerNode<T> node) : base(node)
            {

            }
        }
    }
}
