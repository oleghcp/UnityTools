using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.AiSimulation.Simple;
using UnityUtilityEditor.Drawers.Attributes;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers.AiSimulation
{
    [CustomPropertyDrawer(typeof(StateCondition), true)]
    internal class StateConditionDrawer : ReferenceSelectionDrawer
    {
        protected override void DrawExtendedContent(in Rect position, SerializedProperty property)
        {
            if (!property.HasManagedReferenceValue())
            {
                Draw(position, property);
                return;
            }

            const float weightFactor = 0.75f;

            Rect rect = position;
            rect.width *= weightFactor;
            Draw(rect, property);

            SerializedProperty notProp = property.FindPropertyRelative(StateCondition.NotFieldName);

            rect.x += rect.width;
            rect.width = position.width * (1f - weightFactor);
            notProp.boolValue = EditorGui.ToggleButton(rect, "Not", notProp.boolValue);
        }

        private void Draw(in Rect position, SerializedProperty property)
        {
            Type conditionType = (property.serializedObject.targetObject as AiStateSet).GetConditionRootType();
            if (conditionType == typeof(StateCondition))
                DrawContent(position, property, conditionType);
            else
                DrawContent(position, property, conditionType, typeof(Any), typeof(All));
        }
    }
}
