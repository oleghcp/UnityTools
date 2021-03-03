using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(ObjectNameAttribute))]
    public class ObjectNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(fieldInfo).GetTypeCode() != TypeCode.String)
            {
                EditorScriptUtility.DrawWrongTypeMessage(position, label, $"Use {nameof(ObjectNameAttribute)} only with strings.");
                return;
            }

            string rootObjectName = property.serializedObject.targetObject.name;

            if (property.stringValue != rootObjectName)
                property.stringValue = rootObjectName;

            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}
