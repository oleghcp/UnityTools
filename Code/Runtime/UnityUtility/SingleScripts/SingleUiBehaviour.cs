using UnityEngine;

namespace UnityUtility.SingleScripts
{
    /// <summary>
    /// SingleScript with RectTransform.
    /// </summary>
    public abstract class SingleUiBehaviour<T> : SingleBehaviour<T> where T : SingleUiBehaviour<T>
    {
#pragma warning disable IDE1006
        public RectTransform rectTransform => transform as RectTransform;
#pragma warning restore IDE1006
    }
}
