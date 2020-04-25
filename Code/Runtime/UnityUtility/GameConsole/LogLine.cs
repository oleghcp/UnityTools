using UnityEngine;
using UnityEngine.UI;
using UnityUtility.Collections;

#pragma warning disable CS0649
namespace UnityUtility.GameConsole
{
    internal class LogLine : UiMonoBehaviour, Poolable
    {
        [SerializeField]
        private Text _text;

        public Text Text
        {
            get { return _text; }
        }

        void Poolable.Reinit()
        {
            gameObject.SetActive(true);
        }

        void Poolable.CleanUp()
        {
            _text.text = string.Empty;
            rectTransform.anchoredPosition = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
