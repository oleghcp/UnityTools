using UnityEngine;

namespace UU
{
    public abstract class UIScript : Script
    {
        public RectTransform rectTransform
        {
            get { return transform as RectTransform; }
        }
    }
}
