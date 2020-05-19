using UnityEngine;

namespace UnityUtility.Scripts
{
    /// <summary>
    /// SingleScript with RectTransform.
    /// </summary>
    public abstract class SingleUiBehaviour<T> : SingleBehaviour<T> where T : SingleUiBehaviour<T>
    {
        public RectTransform rectTransform
        {
            get { return transform as RectTransform; }
        }
    }
}
