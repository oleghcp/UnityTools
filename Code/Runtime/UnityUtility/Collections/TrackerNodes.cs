using System;

namespace UnityUtility.Collections
{
    public interface INode
    {
        bool Changed { get; }
    }

    internal static class TrackerNodes
    {
        public interface IActiveNode : INode
        {
            void Check();
            void Force();
        }

        // -- //

        public abstract class NodeDecorator : IActiveNode
        {
            private IActiveNode m_prevNode;

            public abstract bool Changed { get; }

            public NodeDecorator(IActiveNode prevNode)
            {
                m_prevNode = prevNode;
            }

            public void Check()
            {
                m_prevNode.Check();
                InnerCheck();
            }

            public void Force()
            {
                m_prevNode.Force();
                InnerForce();
            }

            protected abstract void InnerCheck();
            protected abstract void InnerForce();
        }

        // -- //

        public class NodeForValueType<T> : NodeDecorator where T : struct, IEquatable<T>
        {
            private Func<T> m_getter;
            private Action m_onChangedCallback;

            private T m_cache;
            private bool m_changed;

            public override bool Changed => m_changed;

            public NodeForValueType(IActiveNode prevNode, Func<T> getter, Action onChangedCallback) : base(prevNode)
            {
                m_getter = getter;
                m_onChangedCallback = onChangedCallback;
            }

            protected override void InnerCheck()
            {
                T tmp = m_getter();

                if (m_changed = !m_cache.Equals(tmp))
                {
                    m_cache = tmp;
                    m_onChangedCallback?.Invoke();
                }
            }

            protected override void InnerForce()
            {
                m_cache = m_getter();
                m_onChangedCallback?.Invoke();
            }
        }

        // -- //

        public class NodeForRefType<T> : NodeDecorator where T : class
        {
            private Func<T> m_getter;
            private Action m_onChangedCallback;

            private T m_cachedObject;

            private bool m_changed;

            public override bool Changed => m_changed;

            public NodeForRefType(IActiveNode prevNode, Func<T> getter, Action onChangedCallback) : base(prevNode)
            {
                m_getter = getter;
                m_onChangedCallback = onChangedCallback;
            }

            protected override void InnerCheck()
            {
                T tmp = m_getter();

                if (m_changed = m_cachedObject != tmp)
                {
                    m_cachedObject = tmp;
                    m_onChangedCallback?.Invoke();
                }
            }

            protected override void InnerForce()
            {
                m_cachedObject = m_getter();
                m_onChangedCallback?.Invoke();
            }
        }

        // -- //

        public class DependentNode : NodeDecorator
        {
            private Action m_onChangedCallback;
            private INode[] m_dependencies;

            public override bool Changed
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

            public DependentNode(IActiveNode prevNode, Action onChangedCallback, INode[] dependencies) : base(prevNode)
            {
                m_onChangedCallback = onChangedCallback;
                m_dependencies = dependencies;
            }

            protected override void InnerCheck()
            {
                if (Changed)
                    m_onChangedCallback();
            }

            protected override void InnerForce()
            {
                m_onChangedCallback();
            }
        }
    }
}
