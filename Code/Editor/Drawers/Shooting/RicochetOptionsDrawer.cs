#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using OlegHcp.Mathematics;
using OlegHcp.Shooting;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(RicochetOptions))]
    internal class RicochetOptionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty countProp = property.FindPropertyRelative(RicochetOptions.CountFieldName);
            SerializedProperty maskProp = property.FindPropertyRelative(RicochetOptions.MaskFieldName);
            SerializedProperty lossProp = property.FindPropertyRelative(RicochetOptions.LossFieldName);

            Rect linePos = position;
            linePos.height = EditorGUIUtility.singleLineHeight;
            linePos.width *= 0.75f;
            countProp.intValue = EditorGUI.IntField(linePos, label, countProp.intValue).ClampMin(0);

            linePos.x += linePos.width + EditorGuiUtility.StandardHorizontalSpacing;
            linePos.width = position.width - linePos.width - EditorGuiUtility.StandardHorizontalSpacing;

            if (GUI.Button(linePos, "Max"))
                countProp.intValue = int.MaxValue;

            if (countProp.intValue != 0)
            {
                linePos = EditorGuiUtility.GetLinePosition(position, 1);
                EditorGUI.PropertyField(linePos, maskProp);
                linePos = EditorGuiUtility.GetLinePosition(position, 2);
                EditorGUI.PropertyField(linePos, lossProp);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative(RicochetOptions.CountFieldName).intValue != 0)
                return EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 2f;

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
