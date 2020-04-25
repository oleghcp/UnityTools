namespace UnityEngine
{
    public abstract class UiMonoBehaviour : MonoBehaviour
    {
        public RectTransform rectTransform
        {
            get { return transform as RectTransform; }
        }
    }
}
