using System;
using OlegHcp.NumericEntities;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor
{
    public static class EditorGuiLayout
    {
        #region Vertical Scroll View
        public static float BeginScrollViewVertical(float scrollPosition, params GUILayoutOption[] options)
        {
            return BeginScrollViewVertical(scrollPosition, false, options);
        }

        public static float BeginScrollViewVertical(float scrollPosition, bool alwaysShow, params GUILayoutOption[] options)
        {
            return BeginScrollViewVertical(scrollPosition, GUI.skin.verticalScrollbar, options);
        }

        public static float BeginScrollViewVertical(float scrollPosition, GUIStyle scrollbar, params GUILayoutOption[] options)
        {
            return BeginScrollViewVertical(scrollPosition, false, scrollbar, GUI.skin.scrollView, options);
        }

        public static float BeginScrollViewVertical(float scrollPosition, bool alwaysShow, GUIStyle scrollbar, GUIStyle background, params GUILayoutOption[] options)
        {
            return EditorGUILayout.BeginScrollView(new Vector2(0f, scrollPosition),
                                                   false,
                                                   alwaysShow,
                                                   GUIStyle.none,
                                                   scrollbar,
                                                   background, options).y;
        }
        #endregion

        #region Horizontal Scroll View
        public static float BeginScrollViewHorizontal(float scrollPosition, params GUILayoutOption[] options)
        {
            return BeginScrollViewHorizontal(scrollPosition, false, options);
        }

        public static float BeginScrollViewHorizontal(float scrollPosition, bool alwaysShow, params GUILayoutOption[] options)
        {
            return BeginScrollViewHorizontal(scrollPosition, GUI.skin.horizontalScrollbar, options);
        }

        public static float BeginScrollViewHorizontal(float scrollPosition, GUIStyle scrollbar, params GUILayoutOption[] options)
        {
            return BeginScrollViewHorizontal(scrollPosition, false, scrollbar, GUI.skin.scrollView, options);
        }

        public static float BeginScrollViewHorizontal(float scrollPosition, bool alwaysShow, GUIStyle scrollbar, GUIStyle background, params GUILayoutOption[] options)
        {
            return EditorGUILayout.BeginScrollView(new Vector2(scrollPosition, 0f),
                                                   alwaysShow,
                                                   false,
                                                   scrollbar,
                                                   GUIStyle.none,
                                                   background,
                                                   options).x;
        }
        #endregion

        #region DiapasonField
        public static Diapason DiapasonField(string label, Diapason diapason, float minLimit, float maxLimit, params GUILayoutOption[] options)
        {
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DiapasonField(position, label, diapason, minLimit, maxLimit);
        }

        public static Diapason DiapasonField(GUIContent label, Diapason diapason, float minLimit, float maxLimit, params GUILayoutOption[] options)
        {
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DiapasonField(position, label, diapason, minLimit, maxLimit);
        }

        public static Diapason DiapasonField(string label, Diapason diapason, params GUILayoutOption[] options)
        {
            return DiapasonField(label, diapason, float.NegativeInfinity, float.PositiveInfinity, options);
        }

        public static Diapason DiapasonField(GUIContent label, Diapason diapason, params GUILayoutOption[] options)
        {
            return DiapasonField(label, diapason, float.NegativeInfinity, float.PositiveInfinity, options);
        }
        #endregion

        #region DiapasonIntField
        public static DiapasonInt DiapasonIntField(string label, DiapasonInt diapason, int minLimit, int maxLimit, params GUILayoutOption[] options)
        {
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DiapasonIntField(position, label, diapason, minLimit, maxLimit);
        }

        public static DiapasonInt DiapasonIntField(GUIContent label, DiapasonInt diapason, int minLimit, int maxLimit, params GUILayoutOption[] options)
        {
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DiapasonIntField(position, label, diapason, minLimit, maxLimit);
        }

        public static DiapasonInt DiapasonIntField(string label, DiapasonInt diapason, params GUILayoutOption[] options)
        {
            return DiapasonIntField(label, diapason, int.MinValue, int.MaxValue, options);
        }

        public static DiapasonInt DiapasonIntField(GUIContent label, DiapasonInt diapason, params GUILayoutOption[] options)
        {
            return DiapasonIntField(label, diapason, int.MinValue, int.MaxValue, options);
        }
        #endregion

        #region ToggleButton
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
        #endregion

        #region DropArea
        public static UnityObject[] DropArea(params GUILayoutOption[] options)
        {
            Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropArea(position);
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
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropArea(position, content, style);
        }
        #endregion

        public static int DropDown(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropDown(propertyRect, selectedIndex, displayedOptions);
        }

        public static int DropDown(string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropDown(propertyRect, label, selectedIndex, displayedOptions);
        }

        public static int DropDown(GUIContent label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.DropDown(propertyRect, label, selectedIndex, displayedOptions);
        }

        public static int IntDropDown(int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.IntDropDown(propertyRect, selectedValue, displayedOptions, optionValues);
        }

        public static int IntDropDown(string label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.IntDropDown(propertyRect, label, selectedValue, displayedOptions, optionValues);
        }

        public static int IntDropDown(GUIContent label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.IntDropDown(propertyRect, label, selectedValue, displayedOptions, optionValues);
        }

        public static Enum EnumDropDown(Enum selected, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.EnumDropDown(propertyRect, selected);
        }

        public static Enum EnumDropDown(string label, Enum selected, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.EnumDropDown(propertyRect, label, selected);
        }

        public static Enum EnumDropDown(GUIContent label, Enum selected, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.EnumDropDown(propertyRect, label, selected);
        }

        public static int MaskDropDown(int mask, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.MaskDropDown(propertyRect, mask, displayedOptions);
        }

        public static int MaskDropDown(string label, int mask, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.MaskDropDown(propertyRect, label, mask, displayedOptions);
        }

        public static int MaskDropDown(GUIContent label, int mask, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.MaskDropDown(propertyRect, label, mask, displayedOptions);
        }

        public static Enum FlagsDropDown(Enum flags, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.FlagsDropDown(propertyRect, flags);
        }

        public static Enum FlagsDropDown(string label, Enum flags, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
            return EditorGui.FlagsDropDown(propertyRect, label, flags);
        }

        public static Enum FlagsDropDown(GUIContent label, Enum flags, params GUILayoutOption[] options)
        {
            Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, options);
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

        #region Scopes
        public class HorizontalCenteringScope : GUI.Scope
        {
            public Rect Rect { get; protected set; }

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
            public Rect Rect { get; protected set; }

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

        public class VerticalScrollViewScope : GUI.Scope
        {
            public float ScrollPosition { get; protected set; }

            public VerticalScrollViewScope(float scrollPosition, params GUILayoutOption[] options)
            {
                ScrollPosition = BeginScrollViewVertical(scrollPosition, options);
            }

            public VerticalScrollViewScope(float scrollPosition, bool alwaysShow, params GUILayoutOption[] options)
            {
                ScrollPosition = BeginScrollViewVertical(scrollPosition, alwaysShow, options);
            }

            public VerticalScrollViewScope(float scrollPosition, GUIStyle scrollbar, params GUILayoutOption[] options)
            {
                ScrollPosition = BeginScrollViewVertical(scrollPosition, scrollbar, options);
            }

            public VerticalScrollViewScope(float scrollPosition, bool alwaysShow, GUIStyle scrollbar, GUIStyle background, params GUILayoutOption[] options)
            {
                ScrollPosition = BeginScrollViewVertical(scrollPosition, alwaysShow, scrollbar, background, options);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndScrollView();
            }
        }

        public class HorizontalScrollViewScope : GUI.Scope
        {
            public float ScrollPosition { get; protected set; }

            public HorizontalScrollViewScope(float scrollPosition, params GUILayoutOption[] options)
            {
                ScrollPosition = BeginScrollViewHorizontal(scrollPosition, options);
            }

            public HorizontalScrollViewScope(float scrollPosition, bool alwaysShow, params GUILayoutOption[] options)
            {
                ScrollPosition = BeginScrollViewHorizontal(scrollPosition, alwaysShow, options);
            }

            public HorizontalScrollViewScope(float scrollPosition, GUIStyle scrollbar, params GUILayoutOption[] options)
            {
                ScrollPosition = BeginScrollViewHorizontal(scrollPosition, scrollbar, options);
            }

            public HorizontalScrollViewScope(float scrollPosition, bool alwaysShow, GUIStyle scrollbar, GUIStyle background, params GUILayoutOption[] options)
            {
                ScrollPosition = BeginScrollViewHorizontal(scrollPosition, alwaysShow, scrollbar, background, options);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndScrollView();
            }
        }
        #endregion
    }
}
