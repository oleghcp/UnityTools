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
            if (!EditorScriptUtility.GetFieldType(fieldInfo).Is(typeof(ScriptableObject)))
            {
                EditorScriptUtility.DrawWrongTypeMessage(position, label, $"Use {nameof(DrawObjectFieldsAttribute)} only with ScriptableObject.");
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
                using (SerializedObject serObject = new SerializedObject(property.objectReferenceValue))
                {
                    using (SerializedProperty iterator = serObject.GetIterator())
                    {
                        Rect rect = position;
                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.indentLevel++;

                        for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                        {
                            if (iterator.propertyPath == EditorUtilityExt.SCRIPT_FIELD)
                                continue;

                            EditorGUI.PropertyField(rect, iterator, true);
                            rect.y += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;
                        }

                        EditorGUI.indentLevel--;
                        serObject.ApplyModifiedProperties();
                    }
                }
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null || !property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            float height = 0;

            using (SerializedObject serObject = new SerializedObject(property.objectReferenceValue))
            {
                SerializedProperty iterator = serObject.GetIterator();
                for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                {
                    height += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;
                }
                iterator.Dispose();
            }

            return height;
        }
    }
}
