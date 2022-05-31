#if INCLUDE_UNITY_UI
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityUtility.UI
{
    //Based on http://www.sharkbombs.com/2015/02/10/tooltips-with-the-new-unity-ui-ugui/
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, TextArea]
        private string _text;
        [SerializeField]
        private bool _customPosition;
        [SerializeField]
        private Vector2 _anchoredPosition;

        private bool _hovered;

        public string Text
        {
            get => _text;
            set => _text = value;
        }

        public bool CustomPosition
        {
            get => _customPosition;
            set => _customPosition = value;
        }

        public Vector2 Position
        {
            get => _anchoredPosition;
            set => _anchoredPosition = value;
        }

        public void Refresh()
        {
            if (_hovered)
                TooltipView.I.Refresh(_text);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            TooltipView.I.ShowTooltip(_text, _customPosition ? _anchoredPosition : eventData.position, _customPosition);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            TooltipView.I.HideTooltip();
        }

        private void OnDisable()
        {
            if (_hovered)
                TooltipView.I.HideTooltip();
        }
    }
}
#endif
