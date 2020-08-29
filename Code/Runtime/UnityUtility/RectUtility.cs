using UnityEngine;

namespace UnityUtility
{
    public static class RectUtility
    {
        public static Vector2 GetAnchor(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft: return new Vector2(0f, 1f);
                case TextAnchor.UpperCenter: return new Vector2(0.5f, 1f);
                case TextAnchor.UpperRight: return new Vector2(1f, 1f);
                case TextAnchor.MiddleLeft: return new Vector2(0f, 0.5f);
                case TextAnchor.MiddleCenter: return new Vector2(0.5f, 0.5f);
                case TextAnchor.MiddleRight: return new Vector2(1f, 0.5f);
                case TextAnchor.LowerLeft: return new Vector2(0f, 0f);
                case TextAnchor.LowerCenter: return new Vector2(0.5f, 0f);
                case TextAnchor.LowerRight: return new Vector2(1f, 0f);
                default: throw new UnsupportedValueException(anchor);
            }
        }
    }
}
