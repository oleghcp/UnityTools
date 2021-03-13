using UnityEngine;

namespace UnityEditor
{
    internal static class EditorStylesExt
    {
        public static GUIStyle DropArea { get; }

        static EditorStylesExt()
        {
            DropArea = new GUIStyle(EditorStyles.helpBox);
            DropArea.alignment = TextAnchor.MiddleCenter;
            DropArea.fontSize = 12;
        }
    }
}
