using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.AiSimulation.Simple;
using UnityUtilityEditor.Drawers.Attributes;

namespace UnityUtilityEditor.Drawers.AiSimulation
{
    [CustomPropertyDrawer(typeof(BehaviorState), true)]
    internal class BehaviorStateDrawer : ReferenceSelectionDrawer
    {
        protected override void DrawExtendedContent(in Rect position, SerializedProperty property)
        {
            Type conditionType = (property.serializedObject.targetObject as AiStateSet).GetStateRootType();
            DrawContent(position, property, conditionType);
        }
    }
}
