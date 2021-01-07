﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(SortingLayerIDAttribute))]
    internal class SortingLayerIDDrawer : PropertyDrawer
    {
        private SortingLayer[] m_layers;
        private string[] m_names;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_layers == null)
            {
                if (EditorScriptUtility.GetFieldType(fieldInfo).GetTypeCode() != TypeCode.Int32)
                {
                    EditorScriptUtility.DrawWrongTypeMessage(position, label, $"Use {nameof(SortingLayerIDAttribute)} with int.");
                    return;
                }

                m_layers = SortingLayer.layers;
                m_names = m_layers.Select(itm => itm.name).ToArray();
            }

            int index = m_layers.IndexOf(itm => itm.id == property.intValue).CutBefore(0);
            index = EditorGUI.Popup(position, property.displayName, index, m_names);
            property.intValue = m_layers[index].id;
        }
    }
}
