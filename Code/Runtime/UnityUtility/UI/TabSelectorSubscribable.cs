using UnityEngine.UI;
using UnityEngine;

#pragma warning disable CS0649
namespace UnityUtility.UI
{
    [DisallowMultipleComponent]
    public class TabSelectorSubscribable : AbstractTabSelector
    {
        [SerializeField]
        private Button _button;

        protected override void OnAwake()
        {
            if (_button != null)
                _button.onClick.AddListener(OnClick);
        }

        public void OnButtonClick()
        {
            OnClick();
        }
    }
}
