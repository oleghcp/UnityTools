using System;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(IdentifierAttribute))]
    internal class IdentifierDrawer : AttributeDrawer<IdentifierAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.String)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(IdentifierAttribute)} with String.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = Guid.NewGuid().ToString();

            if (attribute.Editable)
            {
                EditorGUI.PropertyField(position, property, label);
            }
            else
            {
                position = EditorGUI.PrefixLabel(position, label);

                Rect buttonRect = position;
                buttonRect.width = EditorGuiUtility.SmallButtonWidth;
                buttonRect.x -= EditorGuiUtility.SmallButtonWidth + EditorGuiUtility.StandardHorizontalSpacing;

                if (GUI.Button(buttonRect, EditorGuiUtility.TempContent("*", "Copy to clipboard")))
                    GUIUtility.systemCopyBuffer = property.stringValue;

                GUI.Label(position, EditorGuiUtility.TempContent(property.stringValue));
            }
        }
    }
}
