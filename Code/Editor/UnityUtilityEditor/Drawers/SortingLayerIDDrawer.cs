using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;
using UnityUtility.MathExt;
using static UnityEngine.SortingLayer;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(SortingLayerIDAttribute))]
    internal class SortingLayerIDDrawer : PropertyDrawer
    {
        private string[] _names;
        private DrawTool _tool;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_tool == null)
            {
                if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.Int32)
                {
                    EditorGui.ErrorLabel(position, label, $"Use {nameof(SortingLayerIDAttribute)} with int.");
                    return;
                }

                _tool = new DrawTool();
            }

            property.intValue = _tool.Draw(position, label.text, property.intValue);
        }

        public class DrawTool
        {
            private string[] _names;

            public DrawTool()
            {
                _names = layers.Select(itm => itm.name)
                                .ToArray();
            }

            public int Draw(string propertyName, int layerId)
            {
                int index = layers.IndexOf(itm => itm.id == layerId).CutBefore(0);
                index = EditorGuiLayout.DropDown(propertyName, index, _names);
                return layers[index].id;
            }

            public int Draw(Rect position, string propertyName, int layerId)
            {
                int index = layers.IndexOf(itm => itm.id == layerId).CutBefore(0);
                index = EditorGui.DropDown(position, propertyName, index, _names);
                return layers[index].id;
            }
        }
    }
}
