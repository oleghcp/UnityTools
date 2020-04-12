using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility.UI
{
    [DisallowMultipleComponent]
    public class TabGroup : MonoBehaviour
    {
        [SerializeField]
        private bool _activateOnAwake = true;

        private AbstractTabSelector m_current;
        private List<AbstractTabSelector> m_selectors = new List<AbstractTabSelector>();

        public void OnActivate(bool on)
        {
            if (m_current == null)
                return;

            if (on)
                m_current.OnSelect();
            else
                m_current.OnDeselect();
        }

        public void Select(AbstractTabSelector selector)
        {
            if (m_current != null)
                m_current.OnDeselect();

            m_current = selector;
            m_current.OnSelect();
        }

        public void Select(GameObject content)
        {
            var index = m_selectors.IndexOf(item => item.Content == content);

            if (index >= 0)
                Select(m_selectors[index]);
        }

        internal void RegSelector(AbstractTabSelector selector)
        {
            m_selectors.Add(selector);

            if (_activateOnAwake && (m_current == null || selector.DefaultTab))
            {
                selector.OnSelect();
                m_current = selector;
                return;
            }

            selector.OnDeselect();
        }
    }
}
