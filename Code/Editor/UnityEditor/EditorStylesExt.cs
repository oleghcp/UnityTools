using UnityEngine;

namespace UnityEditor
{
    internal static class EditorStylesExt
    {
        public static GUIStyle DropArea { get; }
        public static GUIStyle DropDown { get; }

        static EditorStylesExt()
        {
            DropArea = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12
            };

            DropDown = new GUIStyle("DropDownButton")
            {
                alignment = TextAnchor.MiddleLeft,
                contentOffset = new Vector2(2f, 0f)
            };
        }
    }
}
