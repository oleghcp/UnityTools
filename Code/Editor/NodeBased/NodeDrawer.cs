using System;
using OlegHcp;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.NodeBased
{
    public class NodeDrawer
    {
        private readonly string _label = "{...}";

        internal Predicate<SerializedProperty> IgnoreCondition;

        protected virtual string ShortDrawingView => _label;
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

        internal void OnGui(SerializedProperty property, float width, bool fullDrawing)
        {
            if (fullDrawing)
            {
                EditorGUIUtility.labelWidth = width * 0.5f;
                OnGui(property, width);
                return;
            }

            GUILayout.Label(ShortDrawingView);
        }

        internal float GetHeight(SerializedProperty property, bool fullDrawing)
        {
            if (fullDrawing)
                return GetHeight(property);

            return EditorGUIUtility.singleLineHeight;
        }

        protected virtual void OnGui(SerializedProperty property, float width)
        {
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
