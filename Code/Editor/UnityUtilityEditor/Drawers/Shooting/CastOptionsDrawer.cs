using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtility.Shooting;

namespace UnityUtilityEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(CastOptions))]
    public class CastOptionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty bounds = property.FindPropertyRelative(nameof(CastOptions.CastBounds));
            SerializedProperty highPrecision = property.FindPropertyRelative(nameof(CastOptions.HighPrecision));

            Rect linePos = position;
            linePos.height = EditorGUIUtility.singleLineHeight;
            bounds.floatValue = EditorGUI.FloatField(linePos, label, bounds.floatValue).CutBefore(0f);

            if (bounds.floatValue > 0f)
            {
                linePos = EditorGuiUtility.GetLinePosition(position, 1);
                EditorGUI.PropertyField(linePos, highPrecision);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative(nameof(CastOptions.CastBounds)).floatValue > 0f)
                return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
