using OlegHcp.Mathematics;
using OlegHcp.NumericEntities;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers
{
    [CustomPropertyDrawer(typeof(SpendingInt))]
    internal class SpendingIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty value = property.FindPropertyRelative(SpendingInt.ValueFieldName);
            SerializedProperty capacity = property.FindPropertyRelative(SpendingInt.CapacityFieldName);
            SpendingDrawerHelper.DrawInt(position, label, value, capacity);
        }
    }

    [CustomPropertyDrawer(typeof(SpendingFloat))]
    internal class SpendingFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty value = property.FindPropertyRelative(SpendingFloat.ValueFieldName);
            SerializedProperty capacity = property.FindPropertyRelative(SpendingFloat.CapacityFieldName);
            SpendingDrawerHelper.DrawFloat(position, label, value, capacity);
        }
    }

    internal static class SpendingDrawerHelper
    {
        private static GUIContent[] _subLabels = { new GUIContent("value:"), new GUIContent("/ ") };

        public static void DrawInt(Rect position, GUIContent label, SerializedProperty a, SerializedProperty b)
        {
            position = EditorGUI.PrefixLabel(position, label);

            int[] array = { a.intValue, b.intValue };

            EditorGUI.MultiIntField(position, _subLabels, array);

            b.intValue = array[1] = array[1].ClampMin(0);
            a.intValue = array[0].Clamp(0, array[1]);
        }

        public static void DrawFloat(Rect position, GUIContent label, SerializedProperty a, SerializedProperty b)
        {
            position = EditorGUI.PrefixLabel(position, label);

            float[] array = { a.floatValue, b.floatValue };

            EditorGUI.MultiFloatField(position, _subLabels, array);

            b.floatValue = array[1] = array[1].ClampMin(0);
            a.floatValue = array[0].Clamp(0, array[1]);
        }
    }
}
