using System;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers
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
            rect.height = EditorGUIUtility.singleLineHeight;
            rect = EditorGUI.PrefixLabel(rect, label);
            DrawExtendedContent(rect, property);

            EditorGui.PropertyFieldIndented(position, property, GUIContent.none, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUI.GetPropertyHeight(property, label, true);

            return EditorGUIUtility.singleLineHeight;
        }

        protected abstract void DrawExtendedContent(in Rect position, SerializedProperty property);
    }
}
