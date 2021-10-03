#if UNITY_2019_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    internal abstract class SerializeReferenceDrawer<TAttribute> : AttributeDrawer<TAttribute> where TAttribute : PropertyAttribute
    {
        private Attribute[] _attributes;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            if (type.IsValueType)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(ReferenceSelectionAttribute)} only with reference types.");
                return;
            }

            if (_attributes == null)
                _attributes = Attribute.GetCustomAttributes(fieldInfo, typeof(SerializeReference));

            if (_attributes.Length == 0)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(SerializeReference)} instead of {nameof(SerializeField)}.");
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            DrawContent(position, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        protected abstract void DrawContent(Rect position, SerializedProperty property);
    }
}
#endif
