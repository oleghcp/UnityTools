using UnityObject = UnityEngine.Object;
using UnityEngine;
using UU;
using UnityEditor;

namespace UUEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawSOFieldsAttribute))]
    public class DrawSOFieldsDrawer : PropertyDrawer
    {
        private float m_height = EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = m_height = EditorGUIUtility.singleLineHeight;

            if (!fieldInfo.FieldType.IsSubclassOf(typeof(ScriptableObject)))
            {
                EditorScriptUtility.DrawWrongTypeMessage(position, label, "Use DrawSOFields with ScriptableObject.");
                return;
            }

            Rect foldPos = position;
            foldPos.width -= EditorGUI.PrefixLabel(position, label).width;
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldPos, property.isExpanded, string.Empty);

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
                    m_height += shift;
                }

                EditorGUI.indentLevel--;
                serObject.ApplyModifiedProperties();
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_height;
        }
    }
}
