using System;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(LayerFieldAttribute))]
    internal class LayerFieldDrawer : AttributeDrawer<LayerFieldAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.Int32)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(LayerFieldAttribute)} with int.");
                return;
            }

            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}
