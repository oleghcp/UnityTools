#if UNITY_2020_1_OR_NEWER && !UNITY_2020_2_OR_NEWER
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(ReorderableArray<>))]
    [CustomPropertyDrawer(typeof(ReorderableRefArray<>))]
    public class DrawnArrayDrawer : PropertyDrawer
    {
        private const string PROP_NAME = "m_array";

        private ReorderableList m_list;
        private string m_label;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_list == null)
            {
                SerializedProperty arrayProperty = property.FindPropertyRelative(PROP_NAME);

                if (arrayProperty == null)
                {
                    EditorGUI.LabelField(position, label);
                    return;
                }

                m_label = label.text;
                m_list = f_createReorderableList(arrayProperty);
            }

            m_list.DoList(position);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_list == null)
                return EditorGUIUtility.singleLineHeight;

            return m_list.GetHeight();
        }

        private ReorderableList f_createReorderableList(SerializedProperty property)
        {
            ReorderableList reordList = new ReorderableList(property.serializedObject, property);

            reordList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, m_label);
            reordList.drawElementCallback += f_onDrawElement;
            reordList.elementHeightCallback += f_onElementHeight;

            return reordList;
        }

        private float f_onElementHeight(int index)
        {
            SerializedProperty elementProperty = m_list.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(elementProperty) + EditorGUIUtility.standardVerticalSpacing;
        }

        private void f_onDrawElement(Rect position, int index, bool isActive, bool isFocused)
        {
            SerializedProperty elementProperty = m_list.serializedProperty.GetArrayElementAtIndex(index);

            const float shift = 10f;

            position.width -= shift;
            position.height = EditorGUIUtility.singleLineHeight;
            position.x += shift;
            position.y += EditorGUIUtility.standardVerticalSpacing * 0.5f;

            EditorGUI.PropertyField(position, elementProperty, true);
        }
    }
}
#endif
