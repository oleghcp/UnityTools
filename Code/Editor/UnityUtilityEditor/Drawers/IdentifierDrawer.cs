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
            if (fieldInfo.FieldType.GetTypeCode() != TypeCode.String)
            {
                EditorScriptUtility.DrawWrongTypeMessage(position, label, "Use Identifier with String.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = Guid.NewGuid().ToString();

            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.LabelField(rect, property.stringValue);
        }
    }
}
