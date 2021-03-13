using System.Runtime.CompilerServices;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class EditorGuiLayout
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CenterButton(string text, params GUILayoutOption[] options)
        {
            return CenterButton(text, GUI.skin.button, options);
        }

        public static bool CenterButton(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool pressed = GUILayout.Button(text, style, options);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return pressed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CenterLabel(string text, params GUILayoutOption[] options)
        {
            CenterLabel(text, GUI.skin.label, options);
        }

        public static void CenterLabel(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(text, style, options);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityObject[] DropArea(string text, params GUILayoutOption[] options)
        {
            return DropArea(text, EditorStylesExt.DropArea, options);
        }

        public static UnityObject[] DropArea(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropArea(position, text, style);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DropDown(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
        {
            return DropDown(null, selectedIndex, displayedOptions, options);
        }

        public static int DropDown(string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropDown(propertyRect, label, selectedIndex, displayedOptions);
        }
    }
}
