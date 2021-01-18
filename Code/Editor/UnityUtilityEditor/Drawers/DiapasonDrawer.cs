using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DiapasonAttribute))]
    public class DiapasonDrawer : PropertyDrawer
    {
        private GUIContent[] _floatSubLabels = { new GUIContent("Min"), new GUIContent("Max") };
        private GUIContent[] _intSubLabels = { new GUIContent("From"), new GUIContent("Before") };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorScriptUtility.GetFieldType(fieldInfo);

            DiapasonAttribute a = attribute as DiapasonAttribute;

            SerializedProperty min = property.FindPropertyRelative("x");
            SerializedProperty max = property.FindPropertyRelative("y");

            if (type.Is(typeof(Vector2)))
            {
                var array = new[] { min.floatValue, max.floatValue };
                position = EditorGUI.PrefixLabel(position, label);
                EditorGUI.MultiFloatField(position, _floatSubLabels, array);
                min.floatValue = array[0].CutBefore(a.MinValue);
                max.floatValue = array[1].CutBefore(array[0]);
                return;
            }

            if (type.Is(typeof(Vector2Int)))
            {
                var array = new[] { min.intValue, max.intValue };
                position = EditorGUI.PrefixLabel(position, label);
                EditorGUI.MultiIntField(position, _intSubLabels, array);
                min.intValue = array[0].CutBefore((int)a.MinValue);
                max.intValue = array[1].CutBefore(array[0] + 1);
                return;
            }

            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.LabelField(rect, $"Use for {nameof(Vector2)} or {nameof(Vector2Int)}.");
            return;
        }
    }
}
