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

        public Text Text
        {
            get { return _text; }
        }

        void IPoolable.Reinit()
        {
            gameObject.SetActive(true);
        }

        void IPoolable.CleanUp()
        {
            _text.text = string.Empty;
            rectTransform.anchoredPosition = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
