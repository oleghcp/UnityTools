#if UNITY_2019_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Window.NodeBased
{
    public class NodeDrawer
    {
        private const string LABEL = "{...}";

        internal Predicate<SerializedProperty> IgnoreCondition;

        protected virtual string ShortDrawingView => LABEL;
        public Predicate<SerializedProperty> IsServiceField => IgnoreCondition;

        internal void OnHeaderGui(bool rootNode, SerializedProperty nameProperty, ref bool renaming)
        {
            if (renaming)
            {
                GUILayout.BeginHorizontal();
                nameProperty.stringValue = EditorGUILayout.TextField(nameProperty.stringValue);
                if (GUILayout.Button("V", GUILayout.Width(EditorGuiUtility.SmallButtonWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    renaming = false;
                GUILayout.EndHorizontal();
                return;
            }

            GUI.color = GetHeaderColor(rootNode);
            EditorGUILayout.LabelField(nameProperty.stringValue, EditorStylesExt.Rect);
            GUI.color = Colours.White;
        }

        internal void OnGui(SerializedProperty property, float width, bool enabled)
        {
            if (enabled)
            {
                OnGui(property, width);
                return;
            }

            EditorGUILayout.LabelField(ShortDrawingView);
        }

        internal float GetHeight(SerializedProperty property, bool enabled)
        {
            if (enabled)
                return GetHeight(property);

            return EditorGUIUtility.singleLineHeight;
        }

        protected virtual void OnGui(SerializedProperty property, float width)
        {
            EditorGUIUtility.labelWidth = width * 0.5f;
            foreach (SerializedProperty item in property.EnumerateInnerProperties())
            {
                if (!IsServiceField(item))
                    EditorGUILayout.PropertyField(item, true);
            }
        }

        protected virtual float GetHeight(SerializedProperty property)
        {
            return EditorGuiUtility.GetDrawHeight(property, IsServiceField);
        }

        protected virtual Color GetHeaderColor(bool rootNode)
        {
            return rootNode ? Colours.Orange : Colours.Cyan;
        }
    }
}
#endif
