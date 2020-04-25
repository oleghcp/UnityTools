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

        private bool m_hovered;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public bool CustomPosition
        {
            get { return _customPosition; }
            set { _customPosition = value; }
        }

        public Vector2 Position
        {
            get { return _anchoredPosition; }
            set { _anchoredPosition = value; }
        }

        public void Refresh()
        {
            if (m_hovered)
                TooltipView.I.Refresh(_text);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            m_hovered = true;
            TooltipView.I.ShowTooltip(_text, _customPosition ? _anchoredPosition : eventData.position, _customPosition);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            m_hovered = false;
            TooltipView.I.HideTooltip();
        }

        private void OnDisable()
        {
            if (m_hovered)
                TooltipView.I.HideTooltip();
        }
    }
}
