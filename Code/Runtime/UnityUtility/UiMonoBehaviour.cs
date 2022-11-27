using UnityEngine;

namespace UnityUtility
{
    public abstract class UiMonoBehaviour : MonoBehaviour
    {
#pragma warning disable IDE1006
        public RectTransform rectTransform => transform as RectTransform;
#pragma warning restore IDE1006
    }
}
