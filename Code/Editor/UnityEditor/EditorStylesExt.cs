using UnityEngine;

namespace UnityEditor
{
    internal static class EditorStylesExt
    {
        public static GUIStyle DropArea { get; }
        public static GUIStyle DropDown { get; }

        static EditorStylesExt()
        {
            DropArea = new GUIStyle(EditorStyles.helpBox);
            DropArea.alignment = TextAnchor.MiddleCenter;
            DropArea.fontSize = 12;

            DropDown = new GUIStyle("DropDownButton");
            DropDown.alignment = TextAnchor.MiddleLeft;
            DropDown.contentOffset = new Vector2(2f, 0f);
        }
    }
}
