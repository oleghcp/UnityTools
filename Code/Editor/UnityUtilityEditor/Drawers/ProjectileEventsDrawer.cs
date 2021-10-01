using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Shooting;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(ProjectileEvents))]
    [CustomPropertyDrawer(typeof(ProjectileEvents2D))]
    internal class ProjectileEventsDrawer : PropertyDrawer
    {
        private const string LABEL = "Use Events";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, EditorGuiUtility.TempContent(LABEL), property);
            DrawSelectionButton(position, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        private void DrawSelectionButton(Rect position, SerializedProperty property)
        {
            float shift = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;

            position.x += shift;
            position.width -= shift;
            position.height = EditorGUIUtility.singleLineHeight;

            bool inited = !property.managedReferenceFullTypename.IsNullOrEmpty();

            bool switched = EditorGUI.Toggle(position, inited);

            if (switched != inited)
            {
                property.serializedObject.Update();

                if (switched)
                {
                    Type fieldType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFieldTypename);
                    property.managedReferenceValue = Activator.CreateInstance(fieldType);
                }
                else
                {
                    property.managedReferenceValue = null;
                }

                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
