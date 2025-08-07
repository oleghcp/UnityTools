#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using OlegHcp.Shooting;
using UnityEditor;
using UnityEngine;
using static OlegHcpEditor.EditorGuiUtility;
using static UnityEditor.EditorGUIUtility;

namespace OlegHcpEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(HitOptions))]
    internal class HitOptionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty reactProp = property.FindPropertyRelative(HitOptions.ReactionFieldName);
            SerializedProperty countProp = property.FindPropertyRelative(HitOptions.CountFieldName);
            SerializedProperty maskProp = property.FindPropertyRelative(HitOptions.MaskFieldName);
            SerializedProperty lossProp = property.FindPropertyRelative(HitOptions.LossFieldName);
            SerializedProperty multProp = property.FindPropertyRelative(HitOptions.MultiplierFieldName);

            Rect linePos = GetLinePosition(position, 0);
            EditorGUI.PropertyField(linePos, reactProp);

            linePos = GetLinePosition(position, 1);
            linePos.width *= 0.75f;

            if (countProp.intValue == 0)
                countProp.intValue = int.MaxValue;
            EditorGUI.PropertyField(linePos, countProp);

            linePos.x += linePos.width + StandardHorizontalSpacing;
            linePos.width = position.width - linePos.width - StandardHorizontalSpacing;
            bool maxPressed = GUI.Button(linePos, "Max");

            linePos = GetLinePosition(position, 2);
            EditorGUI.PropertyField(linePos, maskProp);
            linePos = GetLinePosition(position, 3);
            if (multProp.floatValue == 0)
                multProp.floatValue = 1f - lossProp.floatValue;
            EditorGUI.PropertyField(linePos, multProp);

            if (maxPressed)
                countProp.intValue = int.MaxValue;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 4;
            return singleLineHeight * lineCount + standardVerticalSpacing * (lineCount - 1);
        }
    }
}
#endif
