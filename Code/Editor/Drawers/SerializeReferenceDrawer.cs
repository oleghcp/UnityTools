#if UNITY_2019_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Drawers
{
    internal abstract class SerializeReferenceDrawer : PropertyDrawer
    {
        private Attribute[] _attributes;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            if (type.IsValueType)
            {
                EditorGui.ErrorLabel(position, label, $"Use attribute only with reference types.");
                return;
            }

            if (_attributes == null)
                _attributes = Attribute.GetCustomAttributes(fieldInfo, typeof(SerializeReference));

            if (_attributes.Length == 0)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(SerializeReference)} instead of {nameof(SerializeField)}.");
                return;
            }

            Rect rect = position;
            rect.xMin += Math.Max(EditorGUIUtility.labelWidth, position.width * 0.33333f) + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;

            DrawContent(rect, property);
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.HasManagedReferenceValue() || !property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        protected abstract void DrawContent(in Rect position, SerializedProperty property);
    }
}
#endif
