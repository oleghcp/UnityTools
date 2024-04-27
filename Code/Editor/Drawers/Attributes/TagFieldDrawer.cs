using System;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(TagFieldAttribute))]
    internal class TagFieldDrawer : AttributeDrawer<TagFieldAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.String)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(TagFieldAttribute)} with {nameof(String)}.");
                return;
            }

            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        }
    }
}
