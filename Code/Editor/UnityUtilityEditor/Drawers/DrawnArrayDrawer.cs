using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityUtility.Collections;

#if UNITY_EDITOR && UNITY_2020_1_OR_NEWER
namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawnArray<>))]
    public class DrawnArrayDrawer : PropertyDrawer
    {
        private const string PROP_NAME = "m_array";

        private ReorderableList m_list;

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

                m_list = f_createReorderableList(arrayProperty, label);
            }

            m_list.DoList(position);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_list == null)
                return EditorGUIUtility.singleLineHeight;

            return m_list.GetHeight();
        }

        private ReorderableList f_createReorderableList(SerializedProperty property, GUIContent label)
        {
            ReorderableList reordList = new ReorderableList(property.serializedObject, property);

            reordList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, label);
            reordList.drawElementCallback += f_onDrawElement;
            reordList.elementHeightCallback += f_onElementHeight;

            return reordList;
        }

        private float f_onElementHeight(int index)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void f_onDrawElement(Rect position, int index, bool isActive, bool isFocused)
        {
            SerializedProperty listProperty = m_list.serializedProperty;
            SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(index);

            position.height = EditorGUIUtility.singleLineHeight;
            position.y += EditorGUIUtility.standardVerticalSpacing * 0.5f;

            EditorGUI.PropertyField(position, elementProperty);
        }
    }
}
#endif
