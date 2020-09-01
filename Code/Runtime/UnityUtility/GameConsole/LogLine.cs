using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility.Collections;

#pragma warning disable CS0649
namespace UnityUtility.GameConsole
{
    internal class LogLine : UiMonoBehaviour, IPoolable, IPointerClickHandler
    {
        [SerializeField]
        private Text _text;
        [SerializeField]
        private InfoPanel _infoPanel;

        private string m_info;

        public void SetText(string text, string info, Color color)
        {
            _text.text = text;
            _text.color = color;
            m_info = info;
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
            m_info = null;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (m_info.HasUsefulData())
                _infoPanel.Show(m_info);
        }
    }
}
