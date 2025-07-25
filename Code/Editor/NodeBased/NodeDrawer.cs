﻿using System;
using System.Collections.Generic;
using OlegHcp;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Window.NodeBased;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.NodeBased
{
    public class NodeDrawer
    {
        private readonly string _label = "{...}";
        private ICollection<string> _ignoredFields;
        private Predicate<SerializedProperty> _ignoreCondition;
        private GraphSidePanel _graphSidePanel;

        protected GraphPanelDrawer PanelDrawer => _graphSidePanel.PanelDrawer;
        protected virtual string ShortDrawingView => _label;
        protected Predicate<SerializedProperty> FieldIgnoreCondition => _ignoreCondition;

        internal void SetUp(ICollection<string> ignoredFields, GraphSidePanel graphSidePanel)
        {
            _ignoredFields = ignoredFields;
            _ignoreCondition = IsServiceField;
            _graphSidePanel = graphSidePanel;
        }

        internal void DrawHeader(bool rootNode, SerializedProperty nameProperty, ref bool renaming)
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

        internal void Draw(SerializedProperty node, float width, bool fullDrawing)
        {
            if (fullDrawing)
            {
                EditorGUIUtility.labelWidth = width * 0.5f;
                OnGui(node, width);
                return;
            }

            GUILayout.Label(ShortDrawingView);
        }

        internal float GetHeight(SerializedProperty node, bool fullDrawing)
        {
            if (fullDrawing)
                return GetHeight(node);

            return EditorGUIUtility.singleLineHeight;
        }

        protected virtual void OnGui(SerializedProperty node, float width)
        {
            foreach (SerializedProperty item in node.EnumerateInnerProperties())
            {
                if (!IsServiceField(item))
                    EditorGUILayout.PropertyField(item, true);
            }
        }

        protected virtual float GetHeight(SerializedProperty node)
        {
            return EditorGuiUtility.GetDrawHeight(node, _ignoreCondition);
        }

        protected virtual Color GetHeaderColor(bool rootNode)
        {
            return rootNode ? Colours.Orange : Colours.Cyan;
        }

        protected bool IsServiceField(SerializedProperty property)
        {
            return _ignoredFields.Contains(property.name);
        }
    }
}
