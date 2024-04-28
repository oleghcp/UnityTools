using OlegHcp.Inspector;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(DisableEditingAttribute))]
    internal class DisableEditingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
