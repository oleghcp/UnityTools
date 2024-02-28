using System;
using OlegHcp;
using OlegHcp.Inspector;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(DrawTypenameAttribute))]
    internal class DrawTypenameDrawer : SerializeReferenceDrawer
    {
        protected override void DrawExtendedContent(in Rect position, SerializedProperty property)
        {
            Type assignedType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFullTypename);
            bool nullRef = assignedType == null;
            string label = nullRef ? "Null" : assignedType.Name;
            GUI.color = nullRef ? Colours.Orange : Colours.Lime;
            GUI.Label(position, label);
            GUI.color = Colours.White;
        }
    }
}
