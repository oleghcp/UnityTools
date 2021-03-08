using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility.UI
{
    [DisallowMultipleComponent]
    public class TabGroup : UiMonoBehaviour
    {
        [SerializeField]
        private bool _activateOnAwake = true;

        private AbstractTabSelector _current;
        private List<AbstractTabSelector> _selectors = new List<AbstractTabSelector>();

        public void OnActivate(bool on)
        {
            if (_current == null)
                return;

            if (on)
                _current.OnSelect();
            else
                _current.OnDeselect();
        }

        public void Select(AbstractTabSelector selector)
        {
            if (_current != null)
                _current.OnDeselect();

            _current = selector;
            _current.OnSelect();
        }

        public void Select(GameObject content)
        {
            var index = _selectors.IndexOf(item => item.Content == content);

            if (index >= 0)
                Select(_selectors[index]);
        }

        internal void RegSelector(AbstractTabSelector selector)
        {
            _selectors.Add(selector);

            if (_activateOnAwake && (_current == null || selector.DefaultTab))
            {
                selector.OnSelect();
                _current = selector;
                return;
            }

            selector.OnDeselect();
        }
    }
}
