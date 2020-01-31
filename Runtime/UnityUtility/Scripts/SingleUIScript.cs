using UnityEngine;

namespace UU.Scripts
{
    /// <summary>
    /// SingleScript with RectTransform.
    /// </summary>
    public abstract class SingleUIScript<T> : SingleScript<T> where T : SingleUIScript<T>
    {
        public RectTransform rectTransform
        {
            get { return transform as RectTransform; }
        }
    }
}
