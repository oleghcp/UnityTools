using System;

namespace UU.Collections
{
    public interface Node
    {
        Node Prev { get; }
        bool Extender { get; }
        bool Changed { get; }
        bool RelativeChanged { get; }
        void Check();
        void Force();
    }

    internal static class TrackerNodes
    {
        public interface BaseNode<T> : Node
        {
            T Value { get; }
        }

        // -- //

        public class EmptyNode : Node
        {
            public Node Prev
            {
                get { return null; }
            }

            public bool Extender
            {
                get { return false; }
            }

            public bool Changed
            {
                get { return false; }
            }

            public bool RelativeChanged
            {
                get { return false; }
            }

            void Node.Check() { }
            void Node.Force() { }
        }

        // -- //

        public abstract class NodeDecorator
        {
            private Node m_prevNode;

            public Node Prev
            {
                get { return m_prevNode; }
            }

            public virtual bool Extender
            {
                get { return false; }
            }

            public NodeDecorator(Node prevNode)
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

        public class BaseValueNode<T> : NodeDecorator, BaseNode<T> where T : struct, IEquatable<T>
        {
            private Func<T> m_getFunc;
            private Action<T> m_setFunc;

            private T m_cache;
            private bool m_changed;

            public bool Changed
            {
                get { return m_changed; }
            }

            public bool RelativeChanged
            {
                get { return m_changed; }
            }

            public T Value
            {
                get { return m_cache; }
            }

            public BaseValueNode(Func<T> getFunc, Action<T> setFunc, Node prevNode) : base(prevNode)
            {
                m_getFunc = getFunc;
                m_setFunc = setFunc;
            }

            protected override void InnerCheck()
            {
                T tmp = m_getFunc();

                if (m_changed = !m_cache.Equals(tmp))
                {
                    m_cache = tmp;
                    m_setFunc?.Invoke(tmp);
                }
            }

            protected override void InnerForce()
            {
                m_cache = m_getFunc();
                m_setFunc?.Invoke(m_cache);
            }
        }

        public class BaseObjectNode<T> : NodeDecorator, BaseNode<T> where T : class
        {
            private Func<T> m_getFunc;
            private Action<T> m_setFunc;

            private T m_cachedObject;

            private bool m_changed;

            public bool Changed
            {
                get { return m_changed; }
            }

            public bool RelativeChanged
            {
                get { return m_changed; }
            }

            public T Value
            {
                get { return m_cachedObject; }
            }

            public BaseObjectNode(Func<T> getFunc, Action<T> setFunc, Node prevNode) : base(prevNode)
            {
                m_getFunc = getFunc;
                m_setFunc = setFunc;
            }

            protected override void InnerCheck()
            {
                T tmp = m_getFunc();

                if (m_changed = m_cachedObject != tmp)
                {
                    m_cachedObject = tmp;
                    m_setFunc?.Invoke(tmp);
                }
            }

            protected override void InnerForce()
            {
                m_cachedObject = m_getFunc();
                m_setFunc?.Invoke(m_cachedObject);
            }
        }

        // -- //

        public class ExtenderWithStruct<TParam> : NodeDecorator, BaseNode<TParam> where TParam : struct, IEquatable<TParam>
        {
            private Func<TParam> m_getParamFunc;
            private TParam m_cachedParam;
            private bool m_changed;

            public override bool Extender
            {
                get { return true; }
            }

            public bool Changed
            {
                get { return m_changed; }
            }

            public bool RelativeChanged
            {
                get { return m_changed || Prev.RelativeChanged; }
            }

            public TParam Value
            {
                get { return m_cachedParam; }
            }

            public ExtenderWithStruct(Func<TParam> getParamFunc, Node prevNode) : base(prevNode)
            {
                if (prevNode is EmptyNode)
                    throw new InvalidOperationException("Param checker cannot be added first.");

                m_getParamFunc = getParamFunc;
            }

            protected override void InnerCheck()
            {
                TParam tmpParam = m_getParamFunc();

                if (m_changed = !m_cachedParam.Equals(tmpParam))
                    m_cachedParam = tmpParam;
            }

            protected override void InnerForce()
            {
                m_cachedParam = m_getParamFunc();
            }
        }

        public class ExtenderWithClass<TParam> : NodeDecorator, BaseNode<TParam> where TParam : class
        {
            private Func<TParam> m_getParamFunc;
            private TParam m_cachedParam;
            private bool m_changed;

            public override bool Extender
            {
                get { return true; }
            }

            public bool Changed
            {
                get { return m_changed; }
            }

            public bool RelativeChanged
            {
                get { return m_changed || Prev.RelativeChanged; }
            }

            public TParam Value
            {
                get { return m_cachedParam; }
            }

            public ExtenderWithClass(Func<TParam> getParamFunc, Node prevNode) : base(prevNode)
            {
                if (prevNode is EmptyNode)
                    throw new InvalidOperationException("Param checker cannot be added first.");

                m_getParamFunc = getParamFunc;
            }

            protected override void InnerCheck()
            {
                TParam tmpParam = m_getParamFunc();

                if (m_changed = m_cachedParam != tmpParam)
                    m_cachedParam = tmpParam;
            }

            protected override void InnerForce()
            {
                m_cachedParam = m_getParamFunc();
            }
        }

        public class RelativeExtender : NodeDecorator, Node
        {
            private Node m_relativeTo;

            public bool Changed
            {
                get { return m_relativeTo.Changed; }
            }

            public bool RelativeChanged
            {
                get { return m_relativeTo.Changed || Prev.RelativeChanged; }
            }

            public override bool Extender
            {
                get { return true; }
            }

            public RelativeExtender(Node prevNode, Node dependency) : base(prevNode)
            {
                if (prevNode is EmptyNode)
                    throw new InvalidOperationException("Param checker cannot be added first.");

                m_relativeTo = dependency;
            }

            protected override void InnerCheck() { }

            protected override void InnerForce() { }
        }

        // -- //

        public class RelativeNodeToPrev<T> : NodeDecorator, BaseNode<T>
        {
            private Action<T> m_setFunc;
            private BaseNode<T> m_relativeTo;

            public bool Changed
            {
                get { return Prev.RelativeChanged; }
            }

            public bool RelativeChanged
            {
                get { return Prev.RelativeChanged; }
            }

            public T Value
            {
                get { return m_relativeTo.Value; }
            }

            public RelativeNodeToPrev(Action<T> setFunc, Node prevNode) : base(prevNode)
            {
                m_setFunc = setFunc;

                Node relative = prevNode;

                while (relative.Extender)
                {
                    relative = relative.Prev;
                }

                BaseNode<T> basicNode = relative as BaseNode<T>;

                if (basicNode == null)
                    throw new InvalidOperationException("There is no basic node.");

                m_relativeTo = basicNode;
            }

            protected override void InnerCheck()
            {
                if (Prev.RelativeChanged)
                    m_setFunc(m_relativeTo.Value);
            }

            protected override void InnerForce()
            {
                m_setFunc(m_relativeTo.Value);
            }
        }

        public class RelativeNodeToTarget<T> : NodeDecorator, BaseNode<T>
        {
            private Action<T> m_setFunc;
            private BaseNode<T> m_relativeTo;

            public bool Changed
            {
                get { return m_relativeTo.Changed; }
            }

            public bool RelativeChanged
            {
                get { return m_relativeTo.Changed; }
            }

            public T Value
            {
                get { return m_relativeTo.Value; }
            }

            public RelativeNodeToTarget(Action<T> setFunc, Node prevNode, Node dependency) : base(prevNode)
            {
                m_setFunc = setFunc;

                BaseNode<T> basicNode = dependency as BaseNode<T>;

                if (basicNode == null)
                    throw new InvalidOperationException("There is no basic node.");

                m_relativeTo = basicNode;
            }

            protected override void InnerCheck()
            {
                if (m_relativeTo.Changed)
                    m_setFunc(m_relativeTo.Value);
            }

            protected override void InnerForce()
            {
                m_setFunc(m_relativeTo.Value);
            }
        }
    }
}
