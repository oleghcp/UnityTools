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
            position = EditorGUI.PrefixLabel(position, label);

            if (attribute.MinValue > attribute.MaxValue)
            {
                EditorGUI.LabelField(position, "Max value cannot be less than min value.");
                return;
            }

            Type type = EditorUtilityExt.GetFieldType(this);

            SerializedProperty min = property.FindPropertyRelative("x");
            SerializedProperty max = property.FindPropertyRelative("y");

            if (!type.Is(typeof(Vector2)))
            {
                EditorGUI.LabelField(position, $"Use for {nameof(Vector2)}.");
                return;
            }

            float[] array = new[] { min.floatValue, max.floatValue };
            EditorGUI.MultiFloatField(position, _floatSubLabels, array);
            min.floatValue = array[0].Clamp(attribute.MinValue, attribute.MaxValue);
            max.floatValue = array[1].Clamp(array[0], attribute.MaxValue);
        }
    }
}
