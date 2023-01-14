using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.Inspector;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers
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
