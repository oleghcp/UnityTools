using System;
using static UnityUtility.Collections.TrackerNodes;

namespace UnityUtility.Collections
{
    public class Tracker : IRefreshable
    {
        private class EmptyNode : IActiveNode
        {
            public bool Changed => false;
            void IActiveNode.Check() { }
            void IActiveNode.Force() { }
        }

        private IActiveNode m_runNode;

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

        public INode AddNodeForValueType<T>(Func<T> getter, Action onChangedCallback = null) where T : struct, IEquatable<T>
        {
            return m_runNode = new NodeForValueType<T>(m_runNode, getter, onChangedCallback);
        }

        public INode AddNodeForRefType<T>(Func<T> getter, Action onChangedCallback = null) where T : class
        {
            return m_runNode = new NodeForRefType<T>(m_runNode, getter, onChangedCallback);
        }

        public INode AddNodeBasedOnPrev(Action onChangedCallback)
        {
            return m_runNode = new DependentNode(m_runNode, onChangedCallback, new[] { m_runNode });
        }

        public INode AddDependentNode(Action onChangedCallback, params INode[] dependencies)
        {
            return m_runNode = new DependentNode(m_runNode, onChangedCallback, dependencies);
        }
    }
}
