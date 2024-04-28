using OlegHcp.Inspector;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(SeparatorAttribute))]
    internal class SeparatorDrawer : DecoratorDrawer<SeparatorAttribute>
    {
        public override void OnGUI(Rect position)
        {
            float halfLineHeight = EditorGUIUtility.singleLineHeight * 0.5f;

            position.yMin += halfLineHeight;
            position.height = attribute.Height;

            EditorGUI.DrawRect(position, attribute.Color);
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight + attribute.Height;
        }
    }
}
