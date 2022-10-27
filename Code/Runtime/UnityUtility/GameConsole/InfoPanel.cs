#if !UNITY_2019_2_OR_NEWER || INCLUDE_UNITY_UI
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityUtility.GameConsole
{
    [DisallowMultipleComponent]
    public class InfoPanel : UiMonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Text _textComponent;

        public void Show(string text)
        {
            _textComponent.text = text;
            gameObject.SetActive(true);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}
#endif
