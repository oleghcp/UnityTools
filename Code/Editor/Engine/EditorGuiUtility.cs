using System;
using System.Collections.Generic;
using OlegHcp.CSharp;
using OlegHcp.Mathematics;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Engine
{
    public static class EditorGuiUtility
    {
        private static GUIContent _commonContent;

        public static string BuiltInSkinsPath => EditorGUIUtility.isProSkin ? "builtin skins/darkskin/images/"
                                                                            : "builtin skins/lightskin/images/";

        public static float SmallButtonWidth => EditorGUIUtility.singleLineHeight + 2f;
        public static float StandardHorizontalSpacing => EditorGUIUtility.standardVerticalSpacing + 1f;
        public static float IndentLevelOffset => 15f;

        public static GUIContent TempContent(string label, string tooltip = null)
        {
            return TempContent(label, null, tooltip);
        }

        public static GUIContent TempContent(Texture2D image, string tooltip = null)
        {
            return TempContent(null, image, tooltip);
        }

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

            return height.ClampMin(EditorGUIUtility.singleLineHeight);
        }

        private static GUIContent TempContent(string label, Texture2D image, string tooltip)
        {
            if (_commonContent == null)
                _commonContent = new GUIContent();

            _commonContent.text = label;
            _commonContent.image = image;
            _commonContent.tooltip = tooltip;
            return _commonContent;
        }
    }
}
