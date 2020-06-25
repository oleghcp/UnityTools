using System;

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

    internal static class TrackerNodes
    {
        public interface IActiveNode : ITrackerNode
        {
            void Check();
            void Force();
        }

        // -- //

        public class NodeForValueType<T> : IActiveNode where T : struct, IEquatable<T>
        {
            private Func<T> m_getter;
            private Action m_onChangedCallback;

            private T m_cache;
            private bool m_changed;

            public bool Changed => m_changed;

            public NodeForValueType(Func<T> getter, Action onChangedCallback)
            {
                m_getter = getter;
                m_onChangedCallback = onChangedCallback;
            }

            public void Check()
            {
                T tmp = m_getter();

                if (m_changed = !m_cache.Equals(tmp))
                {
                    m_cache = tmp;
                    m_onChangedCallback?.Invoke();
                }
            }

            public void Force()
            {
                m_cache = m_getter();
                m_onChangedCallback?.Invoke();
            }
        }

        // -- //

        public class NodeForRefType<T> : IActiveNode where T : class
        {
            private Func<T> m_getter;
            private Action m_onChangedCallback;

            private T m_cachedObject;

            private bool m_changed;

            public bool Changed => m_changed;

            public NodeForRefType(Func<T> getter, Action onChangedCallback)
            {
                m_getter = getter;
                m_onChangedCallback = onChangedCallback;
            }

            public void Check()
            {
                T tmp = m_getter();

                if (m_changed = m_cachedObject != tmp)
                {
                    m_cachedObject = tmp;
                    m_onChangedCallback?.Invoke();
                }
            }

            public void Force()
            {
                m_cachedObject = m_getter();
                m_onChangedCallback?.Invoke();
            }
        }

        // -- //

        public class DependentNode : IActiveNode
        {
            private Action m_onChangedCallback;
            private ITrackerNode[] m_dependencies;

            public bool Changed
            {
                get
                {
                    for (int i = 0; i < m_dependencies.Length; i++)
                    {
                        if (m_dependencies[i].Changed)
                            return true;
                    }

                    return false;
                }
            }

            public DependentNode(Action onChangedCallback, ITrackerNode[] dependencies)
            {
                m_onChangedCallback = onChangedCallback;
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

        // -- //

        public class CustomNodeWrapper : IActiveNode
        {
            private CustomTrackerNode _node;

            public bool Changed => _node.Changed;

            public CustomNodeWrapper(CustomTrackerNode node)
            {
                _node = node;
            }

            public void Check()
            {
                _node.Check();
            }

            public void Force()
            {
                _node.Force();
            }
        }
    }
}
