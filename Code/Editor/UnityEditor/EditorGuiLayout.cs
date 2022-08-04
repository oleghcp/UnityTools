using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class EditorGuiLayout
    {
        public static bool ToggleButton(string text, bool value, params GUILayoutOption[] options)
        {
            return ToggleButton(text, value, GUI.skin.button, options);
        }

        public static bool ToggleButton(string text, bool value, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.Toggle(value, text, style, options);
        }

        public static bool ToggleButton(GUIContent content, bool value, params GUILayoutOption[] options)
        {
            return ToggleButton(content, value, GUI.skin.button, options);
        }

        public static bool ToggleButton(GUIContent content, bool value, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.Toggle(value, content, style, options);
        }

        public static UnityObject[] DropArea(params GUILayoutOption[] options)
        {
            return DropArea(null, options);
        }

        public static UnityObject[] DropArea(string text, params GUILayoutOption[] options)
        {
            return DropArea(text, EditorStylesExt.DropArea, options);
        }

        public static UnityObject[] DropArea(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropArea(position, text, style);
        }

        public static int DropDown(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
        {
            return DropDown(null, selectedIndex, displayedOptions, options);
        }

        public static int DropDown(string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropDown(propertyRect, label, selectedIndex, displayedOptions);
        }

        public static int IntDropDown(int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
        {
            return IntDropDown(null, selectedValue, displayedOptions, optionValues, options);
        }

        public static int IntDropDown(string label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.IntDropDown(propertyRect, label, selectedValue, displayedOptions, optionValues);
        }

        public static Enum EnumDropDown(Enum selected, params GUILayoutOption[] options)
        {
            return EnumDropDown(null, selected, options);
        }

        public static Enum EnumDropDown(string label, Enum selected, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.EnumDropDown(propertyRect, label, selected);
        }

        public static int MaskDropDown(int mask, string[] displayedOptions, params GUILayoutOption[] options)
        {
            return MaskDropDown(null, mask, displayedOptions, options);
        }

        public static int MaskDropDown(string label, int mask, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.MaskDropDown(propertyRect, label, mask, displayedOptions);
        }

        public static Enum FlagsDropDown(Enum flags, params GUILayoutOption[] options)
        {
            return FlagsDropDown(null, flags, options);
        }

        public static Enum FlagsDropDown(string label, Enum flags, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.FlagsDropDown(propertyRect, label, flags);
        }

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

        public static void CenterLabel(string text, params GUILayoutOption[] options)
        {
            CenterLabel(text, EditorStyles.label, options);
        }

        public static void CenterLabel(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(text, style, options);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}
