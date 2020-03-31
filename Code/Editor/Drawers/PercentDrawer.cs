using UnityEngine;
using UnityEditor;
using UnityUtility;

namespace UUEditor.Drawers
{
    [CustomPropertyDrawer(typeof(Percent))]
    internal class PercentDrawer : PropertyDrawer
    {
        private string m_name;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_name == null)
            {
                m_name = property.displayName + " (%)";
            }

            SerializedProperty field = property.FindPropertyRelative(Percent.SerFieldName);
            field.floatValue = EditorGUI.FloatField(position, m_name, field.floatValue / Percent.PERCENT_2_RATIO) * Percent.PERCENT_2_RATIO;
        }
    }
}
