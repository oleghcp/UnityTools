using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityEditor
{
    public static class EditorGuiUtility
    {
        private static GUIContent _commonContent;

        public static float SmallButtonWidth
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => EditorGUIUtility.singleLineHeight + 2f;
        }

        public static float StandardHorizontalSpacing
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => EditorGUIUtility.standardVerticalSpacing + 1f;
        }

        public static GUIContent TempContent(string t, string tooltip = null)
        {
            if (_commonContent == null)
                _commonContent = new GUIContent();

            _commonContent.text = t;
            _commonContent.tooltip = tooltip;
            _commonContent.image = null;
            return _commonContent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetLinePosition(in Rect basePosition, int lineIndex)
        {
            return GetLinePosition(basePosition, lineIndex, EditorGUIUtility.singleLineHeight);
        }

        public static Rect GetLinePosition(in Rect basePosition, int lineIndex, float lineHeight)
        {
            float lineSpace = EditorGUIUtility.standardVerticalSpacing;

            float xPos = basePosition.xMin;
            float yPos = basePosition.yMin + (lineHeight + lineSpace) * lineIndex;

            return new Rect(xPos, yPos, basePosition.width, lineHeight);
        }

        public static Rect GetLinePosition(in Rect basePosition, int line, int column, int columnCount)
        {
            float lineSpace = EditorGUIUtility.standardVerticalSpacing;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float lineWidth = basePosition.width;

            if (columnCount > 1)
            {
                lineWidth -= lineSpace * (columnCount - 1);
                lineWidth /= columnCount;
            }

            float yPos = basePosition.yMin + (lineHeight + lineSpace) * line;
            float xPos = basePosition.xMin;

            if (columnCount > 1)
                xPos += (lineWidth + lineSpace) * column;

            return new Rect(xPos, yPos, lineWidth, lineHeight);
        }

        public static float GetDrawHeight(SerializedObject serializedObject, Predicate<SerializedProperty> ignoreCondition = null)
        {
            return GetDrawHeight(serializedObject.EnumerateProperties(false), ignoreCondition);
        }

        public static float GetDrawHeight(SerializedProperty property, Predicate<SerializedProperty> ignoreCondition = null)
        {
            return GetDrawHeight(property.EnumerateInnerProperties(false), ignoreCondition);
        }

        public static string GetTypeDisplayName(Type type)
        {
            string nameSpace = type.Namespace;
            if (nameSpace.IsNullOrEmpty())
                nameSpace = "no namespace";

            return $"{type.Name} ({nameSpace})";
        }

        private static float GetDrawHeight(IEnumerable<SerializedProperty> properties, Predicate<SerializedProperty> ignoreCondition)
        {
            float height = 0f;

            foreach (SerializedProperty item in properties)
            {
                if (ignoreCondition == null || !ignoreCondition(item))
                    height += EditorGUI.GetPropertyHeight(item) + EditorGUIUtility.standardVerticalSpacing;
            }

            return height.CutBefore(EditorGUIUtility.singleLineHeight);
        }
    }
}
