using UnityEngine;

namespace UnityEditor
{
    public static class EditorStylesExt
    {
        public static GUIStyle DropArea { get; }
        public static GUIStyle DropDown { get; }
        public static GUIStyle Rect { get; }

        static EditorStylesExt()
        {
            string path = EditorGuiUtility.BuiltInSkinsPath;

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

            Rect = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                border = new RectOffset(4, 4, 4, 4)
            };
            Rect.normal.background = Load($"{path}pre button.png");
            Rect.contentOffset = new Vector2(2f, 0f);
        }

        private static Texture2D Load(string path)
        {
            return EditorGUIUtility.Load(path) as Texture2D;
        }
    }
}
