using UnityEngine;

namespace UU.UI
{
    [DisallowMultipleComponent]
    public class TabGroup : MonoBehaviour
    {
        private AbstractTabSelector m_cur;

        private void Start()
        {
            if (m_cur != null)
                m_cur.OnSelect();
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
