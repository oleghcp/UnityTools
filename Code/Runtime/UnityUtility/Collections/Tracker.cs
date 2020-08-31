using System;
using System.Collections.Generic;
using static UnityUtility.Collections.TrackerNodes;

namespace UnityUtility.Collections
{
    public class Tracker : IRefreshable
    {
        private List<IActiveNode> m_nodes = new List<IActiveNode>();

        public void Refresh()
        {
            for (int i = 0; i < m_nodes.Count; i++)
            {
                m_nodes[i].Check();
            }
        }

        public void Force()
        {
            for (int i = 0; i < m_nodes.Count; i++)
            {
                m_nodes[i].Force();
            }
        }

        public void Clear()
        {
            m_nodes.Clear();
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter, Action onChangedCallback = null) where T : struct, IEquatable<T>
        {
            return m_nodes.Place(new NodeForValueType<T>(getter, onChangedCallback, null));
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter, Action<T> onChangedCallback = null) where T : struct, IEquatable<T>
        {
            return m_nodes.Place(new NodeForValueType<T>(getter, null, onChangedCallback));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter, Action onChangedCallback = null) where T : class
        {
            return m_nodes.Place(new NodeForRefType<T>(getter, onChangedCallback, null));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter, Action<T> onChangedCallback = null) where T : class
        {
            return m_nodes.Place(new NodeForRefType<T>(getter, null, onChangedCallback));
        }

        public ITrackerNode AddNodeBasedOnPrev(Action onChangedCallback)
        {
            if (m_nodes.Count == 0)
                throw new InvalidOperationException("Tracker does not contain nodes.");

            return m_nodes.Place(new DependentNode(onChangedCallback, new[] { m_nodes.GetLast() }));
        }

        public ITrackerNode AddDependentNode(Action onChangedCallback, params ITrackerNode[] dependencies)
        {
            return m_nodes.Place(new DependentNode(onChangedCallback, dependencies));
        }

        public ITrackerNode AddCustomNode(CustomTrackerNode node)
        {
            return m_nodes.Place(new CustomNodeWrapper(node));
        }
    }
}
