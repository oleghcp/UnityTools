using System;
using OlegHcp;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Drawers
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            if (!type.IsAssignableTo(typeof(UnityObject)))
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(RequiredAttribute)} with {nameof(UnityEngine)}.{nameof(UnityEngine.Object)}.");
                return;
            }

            GUI.color = property.objectReferenceValue ? Colours.White
                                                      : Colours.LightRed;
            property.Draw(position);
            GUI.color = Colours.White;
        }
    }
}
