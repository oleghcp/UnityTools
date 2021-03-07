using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawObjectFieldsAttribute))]
    public class DrawObjectFieldsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!EditorUtilityExt.GetFieldType(fieldInfo).Is(typeof(ScriptableObject)))
            {
                GUIExt.DrawWrongTypeMessage(position, label, $"Use {nameof(DrawObjectFieldsAttribute)} only with ScriptableObject.");
                return;
            }

            position.height = EditorGUIUtility.singleLineHeight;

            if (property.objectReferenceValue != null)
            {
                Rect foldPos = position;

                if (fieldInfo.FieldType.IsArray)
                    foldPos.x += 16f;

                foldPos.width -= EditorGUI.PrefixLabel(position, label).width;
                property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldPos, property.isExpanded, string.Empty);
            }

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();

            if (property.objectReferenceValue == null)
                return;

            if (property.isExpanded)
            {
                Rect rect = position;
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                using (SerializedObject serObject = new SerializedObject(property.objectReferenceValue))
                {
                    GUIExt.DrawObjectFields(rect, serObject, prop => prop.propertyPath == EditorUtilityExt.SCRIPT_FIELD);
                    serObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null || !property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            using (SerializedObject serObject = new SerializedObject(property.objectReferenceValue))
            {
                return EditorGUIUtilityExt.GetDrawHeight(serObject);
            }
        }
    }
}
