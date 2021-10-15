using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(IdentifierAttribute))]
    internal class IdentifierDrawer : AttributeDrawer<IdentifierAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.String)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(IdentifierAttribute)} with String.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = Guid.NewGuid().ToString();

            if (attribute.Editable)
                EditorGUI.PropertyField(position, property, label);
            else
                EditorGUI.LabelField(position, label, EditorGuiUtility.TempContent(property.stringValue));
        }
    }
}
