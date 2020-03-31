using UnityUtility;
using UnityEditor;
using UnityEngine;

namespace UUEditor.Drawers
{
    [CustomPropertyDrawer(typeof(PercentRangeAttribute))]
    internal class PercentRangeDrawer : PropertyDrawer
    {
        private string m_name;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_name == null)
            {
                if (fieldInfo.FieldType != typeof(Percent))
                {
                    EditorScriptUtility.DrawWrongTypeMessage(position, label, "Use PercentRange with Percent.");
                    return;
                }

                m_name = property.displayName + " (%)";
            }

            PercentRangeAttribute a = attribute as PercentRangeAttribute;
            SerializedProperty field = property.FindPropertyRelative(Percent.SerFieldName);
            field.floatValue = EditorGUI.Slider(position, m_name, field.floatValue / Percent.PERCENT_2_RATIO, a.Min, a.Max) * Percent.PERCENT_2_RATIO;
        }
    }
}
