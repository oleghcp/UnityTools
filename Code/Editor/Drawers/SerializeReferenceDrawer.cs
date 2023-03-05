#if UNITY_2019_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Engine;

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
            DrawExtendedContent(rect, property);

            EditorGUI.PrefixLabel(new Rect(position.position, new Vector2(EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight)), label);
            EditorGUI.PropertyField(position, property, GUIContent.none, true);
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
#endif
