﻿using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Engine;
using static UnityUtility.NumericEntities.RngParam;

namespace Drawers
{
    [CustomPropertyDrawer(typeof(Option))]
    internal class RngParamOptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty modeProp = property.FindPropertyRelative(Option.ModeFieldName);

            Rect lineRect = EditorGuiUtility.GetLinePosition(position, 0);
            EditorGUI.PropertyField(lineRect, modeProp);

            if (modeProp.enumValueIndex == 0)
                return;

            SerializedProperty intProp = property.FindPropertyRelative(Option.IntensityFieldName);
            lineRect = EditorGuiUtility.GetLinePosition(position, 1);
            EditorGUI.PropertyField(lineRect, intProp);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            using (SerializedProperty modeProp = property.FindPropertyRelative(Option.ModeFieldName))
            {
                if (modeProp.enumValueIndex == 0)
                    return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}