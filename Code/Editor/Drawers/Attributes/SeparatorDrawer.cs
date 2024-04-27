using OlegHcp.Inspector;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(SeparatorAttribute))]
    internal class SeparatorDrawer : AttributeDrawer<SeparatorAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = position;
            rect.yMin += EditorGUIUtility.singleLineHeight * 0.25f;
            rect.height = attribute.Height;
            EditorGUI.DrawRect(rect, attribute.Color);

            rect = position;
            rect.yMin += attribute.Height + EditorGUIUtility.singleLineHeight * 0.5f + EditorGUIUtility.standardVerticalSpacing;
            property.Draw(rect, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetHeight(label, true) +
                   attribute.Height +
                   EditorGUIUtility.standardVerticalSpacing +
                   EditorGUIUtility.singleLineHeight * 0.5f;
        }
    }
}
