using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;
using static UnityEngine.SortingLayer;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(SortingLayerIDAttribute))]
    internal class SortingLayerIDDrawer : PropertyDrawer
    {
        private string[] m_names;
        private DrawTool m_tool;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_tool == null)
            {
                if (EditorUtilityExt.GetFieldType(fieldInfo).GetTypeCode() != TypeCode.Int32)
                {
                    EditorScriptUtility.DrawWrongTypeMessage(position, label, $"Use {nameof(SortingLayerIDAttribute)} with int.");
                    return;
                }

                m_tool = new DrawTool();
            }

            property.intValue = m_tool.Draw(position, property.displayName, property.intValue);
        }

        public class DrawTool
        {
            private string[] m_names;

            public DrawTool()
            {
                m_names = layers.Select(itm => itm.name)
                                .ToArray();
            }

            public int Draw(string propertyName, int layerId)
            {
                int index = layers.IndexOf(itm => itm.id == layerId).CutBefore(0);
                index = EditorGUILayout.Popup(propertyName, index, m_names);
                return layers[index].id;
            }

            public int Draw(Rect position, string propertyName, int layerId)
            {
                int index = layers.IndexOf(itm => itm.id == layerId).CutBefore(0);
                index = EditorGUI.Popup(position, propertyName, index, m_names);
                return layers[index].id;
            }
        }
    }
}
