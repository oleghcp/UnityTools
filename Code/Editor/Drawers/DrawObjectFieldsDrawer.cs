using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.Inspector;
using UnityUtility.Tools;
using UnityUtilityEditor.Engine;

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
            Rect lineRect = EditorGuiUtility.GetLinePosition(position, 0);

            EditorGUI.ObjectField(lineRect, property, typeof(ScriptableObject), EditorGuiUtility.TempContent(Helper.SPACE));
            EditorGUI.PrefixLabel(lineRect, label);
            property.isExpanded = EditorGUI.Foldout(lineRect, property.isExpanded, GUIContent.none, true);

            if (property.objectReferenceValue == null || !property.isExpanded)
                return;

            lineRect = EditorGuiUtility.GetLinePosition(position, 1);

            using (SerializedObject serObject = new SerializedObject(property.objectReferenceValue))
            {
                serObject.Update();

                if (attribute.NeedIndent)
                    EditorGUI.indentLevel++;

                foreach (var item in serObject.EnumerateProperties())
                {
                    if (item.propertyPath == EditorUtilityExt.SCRIPT_FIELD)
                        continue;

                    EditorGUI.PropertyField(lineRect, item, true);
                    lineRect.y += EditorGUI.GetPropertyHeight(item) + EditorGUIUtility.standardVerticalSpacing;
                }

                if (attribute.NeedIndent)
                    EditorGUI.indentLevel--;

                serObject.ApplyModifiedProperties();
            }
        }
    }
}
