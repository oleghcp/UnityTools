#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using OlegHcp.Mathematics;
using OlegHcp.Shooting;
using UnityEditor;
using UnityEngine;
using static OlegHcpEditor.EditorGuiUtility;
using static UnityEditor.EditorGUIUtility;

namespace OlegHcpEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(HitOptions))]
    internal class RicochetOptionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty reactProp = property.FindPropertyRelative(HitOptions.ReactionFieldName);
            SerializedProperty countProp = property.FindPropertyRelative(HitOptions.CountFieldName);
            SerializedProperty maskProp = property.FindPropertyRelative(HitOptions.MaskFieldName);
            SerializedProperty lossProp = property.FindPropertyRelative(HitOptions.LossFieldName);

            Rect linePos = GetLinePosition(position, 0);
            EditorGUI.PropertyField(linePos, reactProp);

            linePos = GetLinePosition(position, 1);
            linePos.width *= 0.75f;
            countProp.intValue = EditorGUI.IntField(linePos, label, countProp.intValue).ClampMin(1);

            linePos.x += linePos.width + StandardHorizontalSpacing;
            linePos.width = position.width - linePos.width - StandardHorizontalSpacing;
            bool maxPressed = GUI.Button(linePos, "Max");

            linePos = GetLinePosition(position, 2);
            EditorGUI.PropertyField(linePos, maskProp);
            linePos = GetLinePosition(position, 3);
            EditorGUI.PropertyField(linePos, lossProp);

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
