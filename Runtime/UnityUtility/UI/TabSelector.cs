using UnityEngine;

#pragma warning disable CS0649
namespace UU.UI
{
    public class TabSelector : UIScript
    {
        [SerializeField]
        private GameObject _selected;
        [SerializeField]
        private GameObject _unselected;
        [SerializeField]
        private GameObject _content;

        private TabGroup m_group;
        private bool m_selected;

        private void Awake()
        {
            TabGroup group = transform.parent.GetComponentInParent<TabGroup>();

            if (group != null)
                group.RegSelector(this);
            else
                Msg.Warning("TabGroup component is not found.");
        }

        internal void SetUp(TabGroup group)
        {
            m_group = group;
            f_switch();
        }

        internal void OnSelect()
        {
            m_selected = true;
            f_switch();
        }

        internal void OnDeselect()
        {
            m_selected = false;
            f_switch();
        }

        public void OnClick()
        {
            m_group.OnSectorChosen(this);
        }

        private void f_switch()
        {
            _content.SetActive(m_selected);
            _selected.SetActive(m_selected);
            _unselected.SetActive(!m_selected);
        }
    }
}
