using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtility.Shooting;

namespace UnityUtilityEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(RicochetInfo))]
    public class RicochetInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty count = property.FindPropertyRelative(nameof(RicochetInfo.Count));
            SerializedProperty mask = property.FindPropertyRelative(nameof(RicochetInfo.RicochetMask));
            SerializedProperty remainder = property.FindPropertyRelative(nameof(RicochetInfo.SpeedRemainder));

            Rect linePos = position;
            linePos.height = EditorGUIUtility.singleLineHeight;
            linePos.width *= 0.75f;
            count.intValue = EditorGUI.IntField(linePos, label, count.intValue).CutBefore(0);

            linePos.x += linePos.width + EditorGuiUtility.StandardHorizontalSpacing;
            linePos.width = position.width - linePos.width - EditorGuiUtility.StandardHorizontalSpacing;

            if (GUI.Button(linePos, "Max"))
                count.intValue = int.MaxValue;

            if (count.intValue != 0)
            {
                linePos = EditorGuiUtility.GetLinePosition(position, 1);
                EditorGUI.PropertyField(linePos, mask);
                linePos = EditorGuiUtility.GetLinePosition(position, 2);
                EditorGUI.PropertyField(linePos, remainder);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative(nameof(RicochetInfo.Count)).intValue != 0)
                return EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 2f;

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
