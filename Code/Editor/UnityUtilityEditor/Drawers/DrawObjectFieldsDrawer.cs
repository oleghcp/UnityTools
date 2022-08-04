using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawObjectFieldsAttribute))]
    internal class DrawObjectFieldsDrawer : AttributeDrawer<DrawObjectFieldsAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!EditorUtilityExt.GetFieldType(this).IsAssignableTo(typeof(ScriptableObject)))
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(DrawObjectFieldsAttribute)} only with ScriptableObject.");
                return;
            }

            position.height = EditorGUIUtility.singleLineHeight;

            if (property.objectReferenceValue == null)
                EditorGUI.PropertyField(position, property, label);
            else
                DrawProperty(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null || !property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            using (SerializedObject serObject = new SerializedObject(property.objectReferenceValue))
            {
                return EditorGuiUtility.GetDrawHeight(serObject);
            }
        }

        private void DrawProperty(in Rect position, SerializedProperty property, GUIContent label)
        {
            Rect labelPos = position;
            labelPos.width = EditorGUIUtility.labelWidth;

            Rect fieldRect = position;
            fieldRect.xMin += labelPos.width + EditorGuiUtility.StandardHorizontalSpacing;

            property.isExpanded = GUI.Toggle(labelPos, property.isExpanded, label, EditorStylesExt.DropDown);
            property.objectReferenceValue = EditorGUI.ObjectField(fieldRect, property.objectReferenceValue, typeof(ScriptableObject), false);

            if (property.objectReferenceValue == null || !property.isExpanded)
                return;

            Rect rect = position;
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            using (SerializedObject serObject = new SerializedObject(property.objectReferenceValue))
            {
                serObject.Update();

                if (attribute.NeedIndent)
                    EditorGUI.indentLevel++;

                foreach (var item in serObject.EnumerateProperties())
                {
                    if (item.propertyPath == EditorUtilityExt.SCRIPT_FIELD)
                        continue;

                    EditorGUI.PropertyField(rect, item, true);
                    rect.y += EditorGUI.GetPropertyHeight(item) + EditorGUIUtility.standardVerticalSpacing;
                }

                if (attribute.NeedIndent)
                    EditorGUI.indentLevel--;

                serObject.ApplyModifiedProperties();
            }
        }
    }
}
