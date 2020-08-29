using UnityEngine;
using UnityEngine.UI;
using UnityUtility.Collections;

#pragma warning disable CS0649
namespace UnityUtility.GameConsole
{
    internal class LogLine : UiMonoBehaviour, IPoolable
    {
        [SerializeField]
        private Text _text;

        public void SetText(string text, Color color)
        {
            _text.text = text;
            _text.color = color;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _text.preferredHeight);
            gameObject.SetActive(true);
        }

        public LogLine Reuse()
        {
            gameObject.SetActive(false);
            transform.SetAsLastSibling();
            return this;
        }

        void IPoolable.Reinit()
        {
            transform.SetAsLastSibling();
        }

        void IPoolable.CleanUp()
        {
            gameObject.SetActive(false);
            _text.text = string.Empty;
        }
    }
}
