using System;
using UnityEditor;
using UnityEngine;
using OlegHcp.AiSimulation;
using OlegHcp.AiSimulation.NodeBased;
using OlegHcp.AiSimulation.Simple;
using OlegHcpEditor.Drawers.Attributes;

namespace OlegHcpEditor.Drawers.AiSimulation
{
    [CustomPropertyDrawer(typeof(CompleteHandler), true)]
    internal class CompleteHandlerDrawer : ReferenceSelectionDrawer
    {
        protected override void DrawExtendedContent(in Rect position, SerializedProperty property)
        {
            Type rootType = GetRootType(property);
            DrawContent(position, property, rootType);
        }

        private Type GetRootType(SerializedProperty property)
        {
            if (property.serializedObject.targetObject is AiStateSet set)
                return set.GetCompleteHandlerRootType();

            return (property.serializedObject.targetObject as AiStateGraph).GetCompleteHandlerRootType();
        }
    }
}
