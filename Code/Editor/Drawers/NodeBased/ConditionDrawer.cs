using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;
using UnityUtility.NodeBased.Service;
using UnityUtilityEditor.Drawers.Attributes;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers.NodeBased
{
    [CustomPropertyDrawer(typeof(Condition), true)]
    internal class ConditionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.xMin += 13f;
            rect = EditorGUI.PrefixLabel(rect, label);
            DrawExtendedContent(rect, property);

            EditorGUI.PropertyField(position, property, GUIContent.none, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUI.GetPropertyHeight(property, label, true);

            return EditorGUIUtility.singleLineHeight;
        }

        private void DrawExtendedContent(in Rect position, SerializedProperty property)
        {
            if (!property.HasManagedReferenceValue())
            {
                DrawContent(position, property);
                return;
            }

            const float weight = 0.75f;

            Rect rect = position;
            rect.width *= weight;
            DrawContent(position, property);

            SerializedProperty notProp = property.FindPropertyRelative(Condition.NotFieldName);

            rect.x += rect.width;
            rect.width = position.width * (1f - weight);
            notProp.boolValue = EditorGui.ToggleButton(rect, "Not", notProp.boolValue);
        }

        private void DrawContent(in Rect position, SerializedProperty property)
        {
            Type conditionType = (property.serializedObject.targetObject as RawGraph).GetConditionRootType();
            if (conditionType == typeof(Condition))
                ReferenceSelectionDrawer.DrawContent(position, property, conditionType);
            else
                ReferenceSelectionDrawer.DrawContent(position, property, conditionType, typeof(Any), typeof(All));
        }
    }
}
