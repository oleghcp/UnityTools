#if UNITY_2019_3_OR_NEWER && (INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D)
using UnityEditor;
using UnityEngine;
using UnityUtility.Mathematics;
using UnityUtility.Shooting;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(RicochetOptions))]
    internal class RicochetOptionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty countProp = property.FindPropertyRelative(nameof(RicochetOptions.Count));
            SerializedProperty maskProp = property.FindPropertyRelative(nameof(RicochetOptions.RicochetMask));
            SerializedProperty lossProp = property.FindPropertyRelative(nameof(RicochetOptions.SpeedLoss));

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
            if (property.FindPropertyRelative(nameof(RicochetOptions.Count)).intValue != 0)
                return EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 2f;

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
