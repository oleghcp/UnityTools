#if UNITY_2019_3_OR_NEWER && (INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D)
using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtility.Shooting;

namespace UnityUtilityEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(RicochetOptions))]
    public class RicochetOptionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty count = property.FindPropertyRelative(nameof(RicochetOptions.Count));
            SerializedProperty mask = property.FindPropertyRelative(nameof(RicochetOptions.RicochetMask));
            SerializedProperty remainder = property.FindPropertyRelative(nameof(RicochetOptions.SpeedRemainder));

            Rect linePos = position;
            linePos.height = EditorGUIUtility.singleLineHeight;
            linePos.width *= 0.75f;
            count.intValue = EditorGUI.IntField(linePos, label, count.intValue).ClampMin(0);

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
            if (property.FindPropertyRelative(nameof(RicochetOptions.Count)).intValue != 0)
                return EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 2f;

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
