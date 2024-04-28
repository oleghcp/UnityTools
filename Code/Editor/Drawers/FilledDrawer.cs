using OlegHcp.NumericEntities;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers
{
    [CustomPropertyDrawer(typeof(FilledInt))]
    internal class FilledIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty filler = property.FindPropertyRelative(FilledInt.FillerFieldName);
            SerializedProperty threshold = property.FindPropertyRelative(FilledInt.ThresholdFieldName);
            SpendingDrawerHelper.DrawInt(position, label, filler, threshold);
        }
    }

    [CustomPropertyDrawer(typeof(FilledFloat))]
    internal class FilledFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty filler = property.FindPropertyRelative(FilledFloat.FillerFieldName);
            SerializedProperty threshold = property.FindPropertyRelative(FilledFloat.ThresholdFieldName);
            SpendingDrawerHelper.DrawFloat(position, label, filler, threshold);
        }
    }
}
