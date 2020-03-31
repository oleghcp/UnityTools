using UnityObject = UnityEngine.Object;

using System;
using UnityEngine;
using UnityEditor;
using UnityUtility;

namespace UUEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawSOFieldsAttribute))]
    public class DrawSOFieldsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = fieldInfo.FieldType;
            Type soType = typeof(ScriptableObject);
            bool canDraw = type.Is(soType) || (type.IsArray && type.GetElementType().Is(soType));

            if (!canDraw)
            {
                EditorScriptUtility.DrawWrongTypeMessage(position, label, "Use DrawSOFields with ScriptableObject.");
                return;
            }

            position.height = EditorGUIUtility.singleLineHeight;

            if (property.objectReferenceValue != null)
            {
                Rect foldPos = position;

                if (type.IsArray)
                    foldPos.x += 16f;

                foldPos.width -= EditorGUI.PrefixLabel(position, label).width;
                property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldPos, property.isExpanded, string.Empty);
            }

            EditorGUI.PropertyField(position, property);

            if (property.objectReferenceValue == null)
                return;

            if (property.isExpanded)
            {
                SerializedObject serObject = new SerializedObject(property.objectReferenceValue);
                SerializedProperty iterator = serObject.GetIterator();

                Rect rect = position;
                rect.y += EditorGUIUtility.singleLineHeight;

                EditorGUI.indentLevel++;

                for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                {
                    if (iterator.propertyPath == "m_Script")
                        continue;

                    EditorGUI.PropertyField(rect, iterator, true);
                    float shift = EditorGUI.GetPropertyHeight(iterator);
                    rect.y += shift;
                }

                EditorGUI.indentLevel--;
                serObject.ApplyModifiedProperties();
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null && property.isExpanded)
            {
                SerializedObject serObject = new SerializedObject(property.objectReferenceValue);
                SerializedProperty iterator = serObject.GetIterator();
                float height = 0;

                for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                {
                    height += EditorGUI.GetPropertyHeight(iterator);
                }

                return height;
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
