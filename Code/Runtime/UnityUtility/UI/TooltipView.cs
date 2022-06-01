#if !UNITY_2019_2_OR_NEWER || INCLUDE_UNITY_UI
using UnityEngine;
using UnityEngine.UI;
using UnityUtility.SingleScripts;

#pragma warning disable CS0649
namespace UnityUtility.UI
{
    //Based on http://www.sharkbombs.com/2015/02/10/tooltips-with-the-new-unity-ui-ugui/
    public class TooltipView : SingleUiBehaviour<TooltipView>
    {
        [SerializeField]
        private Text _tooltipText;
        [SerializeField]
        private float _frameOffset;

        public bool IsActive => gameObject.activeSelf;

        public float FrameOffset
        {
            get => _frameOffset;
            set => _frameOffset = value;
        }

        protected override void Construct()
        {
            HideTooltip();
        }

        protected override void CleanUp() { }

        public void ShowTooltip(string text, Vector2 position, bool customPosition)
        {
            Refresh(text);

            if (customPosition)
                rectTransform.anchoredPosition = position;
            else
                rectTransform.position = position;

            gameObject.SetActive(true);
        }

        public void Refresh(string text)
        {
            if (_tooltipText.text != text)
            {
                _tooltipText.text = text;

                Vector2 size = new Vector2
                {
                    x = _tooltipText.preferredWidth + _frameOffset,
                    y = _tooltipText.preferredHeight + _frameOffset,
                };

                rectTransform.SetSizeWithCurrentAnchors(size);
            }
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }
    }
}
#endif
