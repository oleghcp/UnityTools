using OlegHcp.Inspector;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    internal class LabelAttributeDrawer : AttributeDrawer<LabelAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.Draw(position, EditorGuiUtility.TempContent(attribute.Label), true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetHeight(label, true);
        }
    }
}
