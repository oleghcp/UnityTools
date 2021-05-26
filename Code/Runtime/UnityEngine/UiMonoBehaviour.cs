namespace UnityEngine
{
    public abstract class UiMonoBehaviour : MonoBehaviour
    {
        public RectTransform rectTransform => transform as RectTransform;
    }
}
