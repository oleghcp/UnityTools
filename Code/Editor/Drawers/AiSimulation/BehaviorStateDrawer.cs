using System;
using OlegHcp.AiSimulation.Simple;
using OlegHcpEditor.Drawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.AiSimulation
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
