#if UNITY_2019_3_OR_NEWER && (INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D)
using UnityEditor;
using UnityEngine;
using UnityUtility.Mathematics;
using UnityUtility.Shooting;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(CastOptions))]
    internal class CastOptionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty radius = property.FindPropertyRelative(nameof(CastOptions.CastRadius));
            SerializedProperty highPrecision = property.FindPropertyRelative(nameof(CastOptions.HighPrecision));

            Rect linePos = position;
            linePos.height = EditorGUIUtility.singleLineHeight;

            float prevValue = radius.floatValue;
            radius.floatValue = EditorGUI.FloatField(linePos, label, radius.floatValue).ClampMin(0f);

            if (radius.floatValue > 0f)
            {
                if (prevValue == 0f)
                    highPrecision.boolValue = true;

                linePos = EditorGuiUtility.GetLinePosition(position, 1);
                EditorGUI.PropertyField(linePos, highPrecision);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative(nameof(CastOptions.CastRadius)).floatValue > 0f)
                return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
