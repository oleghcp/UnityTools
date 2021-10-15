using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DiapasonAttribute))]
    internal class DiapasonDrawer : AttributeDrawer<DiapasonAttribute>
    {
        private GUIContent[] _floatSubLabels = { new GUIContent("Min"), new GUIContent("Max") };
        private GUIContent[] _intSubLabels = { new GUIContent("From"), new GUIContent("Before") };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            SerializedProperty min = property.FindPropertyRelative("x");
            SerializedProperty max = property.FindPropertyRelative("y");

            if (type.Is(typeof(Vector2)))
            {
                var array = new[] { min.floatValue, max.floatValue };
                position = EditorGUI.PrefixLabel(position, label);
                EditorGUI.MultiFloatField(position, _floatSubLabels, array);
                min.floatValue = array[0].CutBefore(attribute.MinValue);
                max.floatValue = array[1].CutBefore(array[0]);
                return;
            }

            if (type.Is(typeof(Vector2Int)))
            {
                var array = new[] { min.intValue, max.intValue };
                position = EditorGUI.PrefixLabel(position, label);
                EditorGUI.MultiIntField(position, _intSubLabels, array);
                min.intValue = array[0].CutBefore((int)attribute.MinValue);
                max.intValue = array[1].CutBefore(array[0] + 1);
                return;
            }

            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.LabelField(rect, $"Use for {nameof(Vector2)} or {nameof(Vector2Int)}.");
            return;
        }
    }
}
