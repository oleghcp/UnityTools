using OlegHcp.Mathematics;
using OlegHcp.NumericEntities;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers
{
    [CustomPropertyDrawer(typeof(AccumInt))]
    internal class AccumIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty got = property.FindPropertyRelative(AccumInt.GotFieldName);
            got.intValue = EditorGUI.IntField(position, label, got.intValue).ClampMin(0);
        }
    }

    [CustomPropertyDrawer(typeof(AccumFloat))]
    internal class AccumFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty got = property.FindPropertyRelative(AccumFloat.GotFieldName);
            got.floatValue = EditorGUI.FloatField(position, label, got.floatValue).ClampMin(0f);
        }
    }
}
