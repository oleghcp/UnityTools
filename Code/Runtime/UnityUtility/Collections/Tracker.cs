using System;
using static UnityUtility.Collections.TrackerNodes;

namespace UnityUtility.Collections
{
    public class Tracker : Refreshable
    {
        private class EmptyNode : ActiveNode
        {
            public bool Changed => false;
            void ActiveNode.Check() { }
            void ActiveNode.Force() { }
        }

        private ActiveNode m_runNode;

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

        public Node AddNodeForValueType<T>(Func<T> getter, Action onChangedCallback = null) where T : struct, IEquatable<T>
        {
            return m_runNode = new NodeForValueType<T>(m_runNode, getter, onChangedCallback);
        }

        public Node AddNodeForRefType<T>(Func<T> getter, Action onChangedCallback = null) where T : class
        {
            return m_runNode = new NodeForRefType<T>(m_runNode, getter, onChangedCallback);
        }

        public Node AddNodeBasedOnPrev(Action onChangedCallback)
        {
            return m_runNode = new DependentNode(m_runNode, onChangedCallback, new[] { m_runNode });
        }

        public Node AddDependentNode(Action onChangedCallback, params Node[] dependencies)
        {
            return m_runNode = new DependentNode(m_runNode, onChangedCallback, dependencies);
        }
    }
}
