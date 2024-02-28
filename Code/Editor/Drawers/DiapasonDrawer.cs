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
            DiapasonDrawerHelper.DrawFloat(position, property, label);
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
        private static GUIContent[] _subLabels = { new GUIContent("Min"), new GUIContent("Max") };

        public static GUIContent[] SubLabels => _subLabels;

        public static void DrawInt(Rect position, SerializedProperty property, GUIContent label, int min = int.MinValue, int max = int.MaxValue)
        {
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty minProp = property.FindPropertyRelative(_subLabels[0].text);
            SerializedProperty maxProp = property.FindPropertyRelative(_subLabels[1].text);

            int[] array = { minProp.intValue, maxProp.intValue };

            EditorGUI.MultiIntField(position, _subLabels, array);

            minProp.intValue = array[0] = array[0].Clamp(min, max);
            maxProp.intValue = array[1].Clamp(array[0], max);
        }

        public static void DrawFloat(Rect position, SerializedProperty property, GUIContent label, float min = float.NegativeInfinity, float max = float.PositiveInfinity)
        {
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty minProp = property.FindPropertyRelative(_subLabels[0].text);
            SerializedProperty maxProp = property.FindPropertyRelative(_subLabels[1].text);

            float[] array = { minProp.floatValue, maxProp.floatValue };

            EditorGUI.MultiFloatField(position, _subLabels, array);

            minProp.floatValue = array[0] = array[0].Clamp(min, max);
            maxProp.floatValue = array[1].Clamp(array[0], max);
        }
    }
}
