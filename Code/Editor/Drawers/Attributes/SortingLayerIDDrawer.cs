using System;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using OlegHcpEditor.Gui;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(SortingLayerIDAttribute))]
    internal class SortingLayerIDDrawer : PropertyDrawer
    {
        private SortingLayerDrawTool _tool;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_tool == null)
            {
                if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.Int32)
                {
                    EditorGui.ErrorLabel(position, label, $"Use {nameof(SortingLayerIDAttribute)} with int.");
                    return;
                }

                _tool = new SortingLayerDrawTool();
            }

            property.intValue = _tool.Draw(position, label.text, property.intValue);
        }
    }
}
