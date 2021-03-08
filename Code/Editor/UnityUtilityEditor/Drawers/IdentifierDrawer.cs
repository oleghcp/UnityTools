using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(IdentifierAttribute))]
    public class IdentifierDrawer : AttributeDrawer<IdentifierAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(fieldInfo).GetTypeCode() != TypeCode.String)
            {
                GUIExt.DrawWrongTypeMessage(position, label, $"Use {nameof(IdentifierAttribute)} with String.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = Guid.NewGuid().ToString();

            if (attribute.Editable)
            {
                EditorGUI.PropertyField(position, property);
                return;
            }

            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.LabelField(rect, property.stringValue);
        }
    }
}
