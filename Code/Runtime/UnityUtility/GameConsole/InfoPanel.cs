#if INCLUDE_UNITY_UI
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OlegHcp.GameConsole
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
