using UnityEngine;

namespace UnityUtility
{
    public abstract class UIScript : Script
    {
        public RectTransform rectTransform
        {
            get { return transform as RectTransform; }
        }
    }
}
