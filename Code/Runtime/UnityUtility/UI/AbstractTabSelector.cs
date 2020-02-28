using UnityEngine;

#pragma warning disable CS0649
namespace UU.UI
{
    public abstract class AbstractTabSelector : MonoBehaviour
    {
        [SerializeField]
        private GameObject _selected;
        [SerializeField]
        private GameObject _unselected;
        [SerializeField]
        private GameObject _content;

        private TabGroup m_group;

        private void Awake()
        {
            TabGroup group = transform.parent.GetComponentInParent<TabGroup>();

            if (group != null)
                group.RegSelector(this);
            else
                Msg.Warning("TabGroup component is not found.");

            OnAwake();
        }

        internal void SetUp(TabGroup group, bool isActive)
        {
            m_group = group;
            f_switch(isActive);
        }

        internal void OnSelect()
        {
            f_switch(true);
        }

        internal void OnDeselect()
        {
            f_switch(false);
        }

        protected virtual void OnAwake() { }

        protected void OnClick()
        {
            m_group.OnSectorChosen(this);
        }

        private void f_switch(bool select)
        {
            _content.SetActive(select);
            _selected.SetActive(select);
            _unselected.SetActive(!select);
            transform.SetAsLastSibling();
        }
    }
}
