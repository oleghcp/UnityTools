﻿#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility.AiSimulation;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(StateCondition))]
    internal class StateConditionDrawer : ReferenceSelectionDrawer
    {
        protected override void DrawContent(in Rect position, SerializedProperty property)
        {
            if (!property.HasManagedReferenceValue())
            {
                base.DrawContent(position, property);
                return;
            }

            const float weight = 0.75f;

            Rect rect = position;
            rect.width *= weight;
            base.DrawContent(rect, property);

            SerializedProperty notProp = property.FindPropertyRelative(StateCondition.NotFieldName);

            rect.x += rect.width;
            rect.width = position.width * (1f - weight);
            notProp.boolValue = EditorGui.ToggleButton(rect, "Not", notProp.boolValue);
        }
    }
}
#endif