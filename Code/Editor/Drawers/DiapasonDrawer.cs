using OlegHcp.Mathematics;
using OlegHcp.NumericEntities;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers
{
    [CustomPropertyDrawer(typeof(Diapason))]
    internal class DiapasonDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DiapasonDrawerHelper.DrawFloat(position, property, label, false);
        }
    }

    [CustomPropertyDrawer(typeof(DiapasonInt))]
    internal class DiapasonIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DiapasonDrawerHelper.DrawInt(position, property, label);
        }
    }

    internal static class DiapasonDrawerHelper
    {
        private static GUIContent[] _subLabels = new[]
        {
            new GUIContent(Diapason.MinFieldName),
            new GUIContent(Diapason.MaxFieldName),
        };

        public static GUIContent[] SubLabels => _subLabels;

        public static void DrawInt(Rect position, SerializedProperty property, GUIContent label, int min = int.MinValue, int max = int.MaxValue)
        {
            SerializedProperty minProp = property.FindPropertyRelative(_subLabels[0].text);
            SerializedProperty maxProp = property.FindPropertyRelative(_subLabels[1].text);

            int[] array = { minProp.intValue, maxProp.intValue };

            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.MultiIntField(position, _subLabels, array);

            minProp.intValue = array[0] = array[0].Clamp(min, max);
            maxProp.intValue = array[1].Clamp(array[0], max);
        }

        public static void DrawFloat(Rect position, SerializedProperty property, GUIContent label, bool slider, float min = float.NegativeInfinity, float max = float.PositiveInfinity)
        {
            SerializedProperty minProp = property.FindPropertyRelative(_subLabels[0].text);
            SerializedProperty maxProp = property.FindPropertyRelative(_subLabels[1].text);

            if (slider)
            {
                DrawWithSlider(position, label, min, max, minProp, maxProp);
                return;
            }

            float[] array = { minProp.floatValue, maxProp.floatValue };
            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.MultiFloatField(position, _subLabels, array);
            minProp.floatValue = array[0] = array[0].Clamp(min, max);
            maxProp.floatValue = array[1].Clamp(array[0], max);
        }

        private static void DrawWithSlider(Rect position, GUIContent label, float min, float max, SerializedProperty minProp, SerializedProperty maxProp)
        {
            float left = minProp.floatValue;
            float right = maxProp.floatValue;

            const float smallWidthFactor = 0.2f;
            position = EditorGUI.PrefixLabel(position, label);

            Rect rect = position;
            rect.width *= smallWidthFactor;
            left = EditorGUI.FloatField(rect, left).Clamp(min, max);

            rect = position;
            rect.xMin += position.width * smallWidthFactor + EditorGuiUtility.StandardHorizontalSpacing;
            rect.xMax -= position.width * smallWidthFactor + EditorGuiUtility.StandardHorizontalSpacing;
            EditorGUI.MinMaxSlider(rect, ref left, ref right, min, max);

            rect = position;
            rect.xMin += rect.width * (1f - smallWidthFactor);
            right = EditorGUI.FloatField(rect, right).Clamp(left, max);

            minProp.floatValue = left;
            maxProp.floatValue = right;
        }
    }
}
