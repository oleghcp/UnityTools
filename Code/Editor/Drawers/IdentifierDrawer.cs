using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(IdentifierAttribute))]
    public class IdentifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = EditorGUI.PrefixLabel(position, label);

            if (fieldInfo.FieldType.GetTypeCode() != TypeCode.String)
            {
                EditorGUI.LabelField(rect, "Use Identifier with String.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = Guid.NewGuid().ToString();

            EditorGUI.LabelField(rect, property.stringValue);
        }
    }
}
