#if !UNITY_2019_2_OR_NEWER || INCLUDE_UNITY_UI
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility.CSharp;
using UnityUtility.Pool;

namespace UnityUtility.GameConsole
{
    [DisallowMultipleComponent]
    internal class LogLine : UiMonoBehaviour, IPoolable, IPointerClickHandler
    {
        [SerializeField]
        private Text _text;
        [SerializeField]
        private InfoPanel _infoPanel;
        [SerializeField, Min(1)]
        private int _initSizeCount = 3;

        private string _info;

        public void SetText(string text, string info, Color color)
        {
            _text.text = text;
            _text.color = color;
            _info = info;
            gameObject.SetActive(true);
            StartCoroutine(GetSizeRoutine());
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
            _info = null;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (_info.HasUsefulData())
                _infoPanel.Show(_info);
        }

        private IEnumerator GetSizeRoutine()
        {
            int couner = 0;

            while (couner++ < _initSizeCount)
            {
                yield return null;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _text.preferredHeight);
            }
        }
    }
}
#endif
