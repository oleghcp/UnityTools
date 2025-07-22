using System;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using OlegHcp.Tools;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(LayerFieldAttribute))]
    internal class LayerFieldDrawer : AttributeDrawer<LayerFieldAttribute>
    {
        private static Action<Rect, SerializedProperty, GUIContent> _layerMaskField = Helper.CreateDelegate<Action<Rect, SerializedProperty, GUIContent>>(typeof(EditorGUI), "LayerMaskField");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.Int32)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(LayerFieldAttribute)} with {nameof(Int32)}.");
                return;
            }

            if (attribute.AsMask)
                _layerMaskField(position, property, label);
            else
                property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}
