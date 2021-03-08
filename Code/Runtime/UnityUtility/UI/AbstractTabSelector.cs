using UnityEngine;

#pragma warning disable CS0649
namespace UnityUtility.UI
{
    public abstract class AbstractTabSelector : UiMonoBehaviour
    {
        [SerializeField]
        private bool _defaultTab;
        [SerializeField]
        private GameObject _selected;
        [SerializeField]
        private GameObject _unselected;
        [SerializeField]
        private GameObject _content;
        [SerializeField, HideInInspector]
        private TabGroup _group;

        public bool DefaultTab => _defaultTab;
        public GameObject Content => _content;

        private void Awake()
        {
            if (_group == null)
                _group = transform.parent.GetComponentInParent<TabGroup>(true);
            
            _group.RegSelector(this);
            OnAwake();
        }

        private void OnValidate()
        {
            _group = transform.parent.GetComponentInParent<TabGroup>(true);
        }

        internal void OnSelect()
        {
            Switch(true);
        }

        internal void OnDeselect()
        {
            Switch(false);
        }

        protected virtual void OnAwake() { }

        protected void OnClick()
        {
            _group.Select(this);
        }

        private void Switch(bool select)
        {
            _content.SetActive(select);
            _selected.SetActive(select);
            _unselected.SetActive(!select);
            transform.SetAsLastSibling();
        }
    }
}
