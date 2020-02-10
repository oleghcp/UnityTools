using System;
using static UU.Collections.TrackerNodes;

namespace UU.Collections
{
    public class Tracker : Refreshable
    {
        private Node m_runNode;

        public Tracker()
        {
            m_runNode = new EmptyNode();
        }

        public void Refresh()
        {
            m_runNode.Check();
        }

        public void Force()
        {
            m_runNode.Force();
        }

        public void Clear()
        {
            m_runNode = new EmptyNode();
        }

        public Node AddBaseValueNode<T>(Func<T> getter, Action<T> setter = null) where T : struct, IEquatable<T>
        {
            return m_runNode = new BaseValueNode<T>(getter, setter, m_runNode);
        }

        public Node AddBaseObjectNode<T>(Func<T> getFunc, Action<T> setter = null) where T : class
        {
            return m_runNode = new BaseObjectNode<T>(getFunc, setter, m_runNode);
        }

        public Node AddStructParam<TParam>(Func<TParam> paramGetter) where TParam : struct, IEquatable<TParam>
        {
            return m_runNode = new ExtenderWithStruct<TParam>(paramGetter, m_runNode);
        }

        public Node AddObjectParam<TParam>(Func<TParam> paramGetter) where TParam : class
        {
            return m_runNode = new ExtenderWithClass<TParam>(paramGetter, m_runNode);
        }

        public Node AddRelativeParam(Node otherNode)
        {
            return m_runNode = new RelativeExtender(m_runNode, otherNode);
        }

        public Node AddRelativeNode<T>(Action<T> setter)
        {
            return m_runNode = new RelativeNodeToPrev<T>(setter, m_runNode);
        }

        public Node AddRelativeNode<T>(Node relativeTo, Action<T> setter)
        {
            return m_runNode = new RelativeNodeToTarget<T>(setter, m_runNode, relativeTo);
        }
    }
}
