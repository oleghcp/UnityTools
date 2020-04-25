using UnityEngine;

namespace UnityUtility
{
    public abstract class UIScript : MonoBehaviour
    {
        public RectTransform rectTransform
        {
            get { return transform as RectTransform; }
        }
    }
}
