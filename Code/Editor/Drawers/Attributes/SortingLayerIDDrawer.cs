using System;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using OlegHcp.Mathematics;
using OlegHcpEditor.Utils;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(SortingLayerIDAttribute))]
    internal class SortingLayerIDDrawer : PropertyDrawer
    {
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
            private SortingLayer[] _layers;

            public DrawTool()
            {
                _layers = SortingLayer.layers;
                _names = new string[_layers.Length + 2];

                for (int i = 0; i < _layers.Length; i++)
                {
                    _names[i] = _layers[i].name;
                }

                _names.FromEnd(0) = "Add Sorting Layer...";
            }

            public int Draw(int layerId)
            {
                return Draw(null, layerId);
            }

            public int Draw(string propertyName, int layerId)
            {
                int index = GetIndex(layerId);
                int newIndex = EditorGuiLayout.DropDown(propertyName, index, _names);
                return CheckIndex(index, newIndex);
            }

            public int Draw(in Rect position, int layerId)
            {
                return Draw(position, null, layerId);
            }

            public int Draw(in Rect position, string propertyName, int layerId)
            {
                int index = GetIndex(layerId);
                int newIndex = EditorGui.DropDown(position, propertyName, index, _names);
                return CheckIndex(index, newIndex);
            }

            private int CheckIndex(int prevIndex, int newIndex)
            {
                if ((uint)newIndex >= (uint)_layers.Length)
                {
                    Selection.activeObject = Managers.GetTagManager();
                    return _layers[prevIndex].id;
                }

                return _layers[newIndex].id;
            }

            private int GetIndex(int layerId)
            {
                return _layers.IndexOf(itm => itm.id == layerId)
                              .Clamp(0, _layers.Length - 1);
            }
        }
    }
}
