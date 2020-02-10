namespace UU.UI
{
    public class TabGroup : UIScript
    {
        private TabSelector m_cur;

        private void Start()
        {
            m_cur?.OnSelect();
        }

        internal void OnSectorChosen(TabSelector selector)
        {
            if (m_cur != null)
                m_cur.OnDeselect();

            m_cur = selector;
            m_cur.OnSelect();
        }

        internal void RegSelector(TabSelector selector)
        {
            if (m_cur == null)
                m_cur = selector;

            selector.SetUp(this);
        }
    }
}
