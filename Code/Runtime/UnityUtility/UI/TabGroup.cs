using UnityEngine;

namespace UnityUtility.UI
{
    [DisallowMultipleComponent]
    public class TabGroup : MonoBehaviour
    {
        private AbstractTabSelector m_cur;

        public void OnActivate(bool on)
        {
            if (m_cur == null)
                return;

            if (on)
                m_cur.OnSelect();
            else
                m_cur.OnDeselect();
        }

        internal void OnSectorChosen(AbstractTabSelector selector)
        {
            if (m_cur != null)
                m_cur.OnDeselect();

            (m_cur = selector).OnSelect();
        }

        internal void RegSelector(AbstractTabSelector selector)
        {
            if (m_cur == null)
                m_cur = selector;

            selector.SetUp(this, m_cur == selector);
        }
    }
}
