using UnityEngine;
using UnityEngine.UI;
using UU.Collections;

#pragma warning disable CS0649
namespace UU.GameConsole
{
    internal class LogLine : UIScript, Poolable
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
