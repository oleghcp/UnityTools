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
            SpendingDrawerHelper.DrawInt(position, property, label);
        }
    }

    [CustomPropertyDrawer(typeof(SpendingFloat))]
    internal class SpendingFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SpendingDrawerHelper.DrawFloat(position, property, label);
        }
    }

    internal static class SpendingDrawerHelper
    {
        private static GUIContent[] _subLabels = { new GUIContent("value:"), new GUIContent("/ ") };

        public static void DrawInt(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty value = property.FindPropertyRelative(SpendingInt.ValueFieldName);
            SerializedProperty capacity = property.FindPropertyRelative(SpendingInt.CapacityFieldName);

            int[] array = { value.intValue, capacity.intValue };

            EditorGUI.MultiIntField(position, _subLabels, array);

            capacity.intValue = array[1] = array[1].ClampMin(0);
            value.intValue = array[0].Clamp(0, array[1]);
        }

        public static void DrawFloat(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty value = property.FindPropertyRelative(SpendingFloat.ValueFieldName);
            SerializedProperty capacity = property.FindPropertyRelative(SpendingFloat.CapacityFieldName);

            float[] array = { value.floatValue, capacity.floatValue };

            EditorGUI.MultiFloatField(position, _subLabels, array);

            capacity.floatValue = array[1] = array[1].ClampMin(0);
            value.floatValue = array[0].Clamp(0, array[1]);
        }
    }
}
