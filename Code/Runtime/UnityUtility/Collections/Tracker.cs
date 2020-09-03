using System;
using System.Collections.Generic;
using UnityUtilityTools;
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

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter) where T : struct, IEquatable<T>
        {
            return m_nodes.Place(new NodeForValueType<T>(getter, null, null));
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter, Action onChangedCallback) where T : struct, IEquatable<T>
        {
            return m_nodes.Place(new NodeForValueType<T>(getter, onChangedCallback, null));
        }

        public ITrackerNode AddNodeForValueType<T>(Func<T> getter, Action<T> onChangedCallback) where T : struct, IEquatable<T>
        {
            return m_nodes.Place(new NodeForValueType<T>(getter, null, onChangedCallback));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter) where T : class
        {
            return m_nodes.Place(new NodeForRefType<T>(getter, null, null));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter, Action onChangedCallback) where T : class
        {
            return m_nodes.Place(new NodeForRefType<T>(getter, onChangedCallback, null));
        }

        public ITrackerNode AddNodeForRefType<T>(Func<T> getter, Action<T> onChangedCallback) where T : class
        {
            return m_nodes.Place(new NodeForRefType<T>(getter, null, onChangedCallback));
        }

        public ITrackerNode AddNodeBasedOnPrev(Action onChangedCallback)
        {
            if (m_nodes.Count == 0)
                throw Errors.EmptyTraker();

            return m_nodes.Place(new DependentNode(onChangedCallback, m_nodes.GetLast(), null));
        }

        public ITrackerNode AddNodeBasedOnPrev<T>(Action<T> onChangedCallback)
        {
            if (m_nodes.Count == 0)
                throw Errors.EmptyTraker();

            return m_nodes.Place(new DependentNodeWithValue<T>(onChangedCallback, m_nodes.GetLast()));
        }

        public ITrackerNode AddDependentNode(Action onChangedCallback, params ITrackerNode[] dependencies)
        {
            return m_nodes.Place(new DependentNode(onChangedCallback, null, dependencies));
        }

        public ITrackerNode AddCustomNode(CustomTrackerNode node)
        {
            return m_nodes.Place(new CustomNodeWrapper(node));
        }

        public ITrackerNode AddCustomNode<T>(CustomTrackerNode<T> node)
        {
            return m_nodes.Place(new CustomNodeWrapper<T>(node));
        }
    }
}
