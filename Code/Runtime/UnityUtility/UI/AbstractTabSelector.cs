using UnityEngine;

#pragma warning disable CS0649
namespace UnityUtility.UI
{
    public abstract class AbstractTabSelector : MonoBehaviour
    {
        [SerializeField]
        private GameObject _selected;
        [SerializeField]
        private GameObject _unselected;
        [SerializeField]
        private GameObject _content;
        [SerializeField, HideInInspector]
        private TabGroup _group;

        private void Awake()
        {
            _group.RegSelector(this);
            OnAwake();
        }

        private void OnValidate()
        {
            _group = transform.parent.GetComponentInParent<TabGroup>(true);

            if (_group == null)
                Debug.LogError("TabGroup component is not found.");
        }

        internal void SetUp(TabGroup group, bool isActive)
        {
            _group = group;
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
            _group.OnSectorChosen(this);
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
