using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Engine
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
            return DropArea((string)null, options);
        }

        public static UnityObject[] DropArea(string text, params GUILayoutOption[] options)
        {
            return DropArea(text, EditorStylesExt.DropArea, options);
        }

        public static UnityObject[] DropArea(GUIContent content, params GUILayoutOption[] options)
        {
            return DropArea(content, EditorStylesExt.DropArea, options);
        }

        public static UnityObject[] DropArea(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            return DropArea(EditorGuiUtility.TempContent(text), style, options);
        }

        public static UnityObject[] DropArea(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropArea(position, content, style);
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

        public static Rect BeginHorizontalCentering(params GUILayoutOption[] options)
        {
            Rect rect = EditorGUILayout.BeginHorizontal(options);
            GUILayout.FlexibleSpace();
            return rect;
        }

        public static Rect BeginHorizontalCentering(GUIStyle style, params GUILayoutOption[] options)
        {
            Rect rect = EditorGUILayout.BeginHorizontal(style, options);
            GUILayout.FlexibleSpace();
            return rect;
        }

        public static void EndHorizontalCentering()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public static Rect BeginVerticalCentering(params GUILayoutOption[] options)
        {
            Rect rect = EditorGUILayout.BeginVertical(options);
            GUILayout.FlexibleSpace();
            return rect;
        }

        public static Rect BeginVerticalCentering(GUIStyle style, params GUILayoutOption[] options)
        {
            Rect rect = EditorGUILayout.BeginVertical(style, options);
            GUILayout.FlexibleSpace();
            return rect;
        }

        public static void EndVerticalCentering()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        public class HorizontalCenteringScope : GUI.Scope
        {
            public Rect Rect { get; private set; }

            public HorizontalCenteringScope(params GUILayoutOption[] options)
            {
                Rect = BeginHorizontalCentering(options);
            }

            public HorizontalCenteringScope(GUIStyle style, params GUILayoutOption[] options)
            {
                Rect = BeginHorizontalCentering(style, options);
            }

            protected override void CloseScope()
            {
                EndHorizontalCentering();
            }
        }

        public class VerticalCenteringScope : GUI.Scope
        {
            public Rect Rect { get; private set; }

            public VerticalCenteringScope(params GUILayoutOption[] options)
            {
                Rect = BeginVerticalCentering(options);
            }

            public VerticalCenteringScope(GUIStyle style, params GUILayoutOption[] options)
            {
                Rect = BeginVerticalCentering(style, options);
            }

            protected override void CloseScope()
            {
                EndVerticalCentering();
            }
        }
    }
}
