using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    //Based on https://forum.unity.com/threads/serializereference-genericserializedreferenceinspectorui.813366/
    [CustomPropertyDrawer(typeof(TypeNameAttribute))]
    internal class TypeNameDrawer : AttributeDrawer<TypeNameAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.String)
            {
                EditorGui.WrongTypeLabel(position, label, $"Use {nameof(TypeNameAttribute)} only with strings.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = attribute.TargetType.GetTypeName();

            EditorGUI.LabelField(position, label);
            DrawSelectionButton(position, property);
        }

        private void DrawSelectionButton(in Rect position, SerializedProperty property)
        {
            float shift = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;

            Rect buttonPosition = position;
            buttonPosition.x += shift;
            buttonPosition.width = position.width - shift;
            buttonPosition.height = EditorGUIUtility.singleLineHeight;

            int storedIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Type savedType = Type.GetType(property.stringValue);

            string shortName;
            string toolTip;

            if (savedType == null)
            {
                shortName = "Unknown";
                toolTip = "TypeName is broken";
                GUI.color = Colours.Red;
            }
            else if (savedType.IsAbstract)
            {
                shortName = $"Type name of {savedType.Name} (Abstract)";
                toolTip = savedType.GetTypeName();
                GUI.color = Colours.Orange;
            }
            else
            {
                shortName = "Type name of " + savedType.Name;
                toolTip = savedType.GetTypeName();

                if (attribute.TargetType == savedType)
                    GUI.color = Colours.Yellow;
            }

            if (GUI.Button(buttonPosition, EditorGuiUtility.TempContent(shortName, toolTip)))
                ShowContextMenu(buttonPosition, property);

            GUI.color = Colours.White;
            EditorGUI.indentLevel = storedIndent;
        }

        private void ShowContextMenu(in Rect buttonPosition, SerializedProperty property)
        {
            DropDownList menu = DropDownList.Create();

            menu.AddItem("Base", () => AssignField(property, attribute.TargetType.GetTypeName()));

            foreach (Type type in TypeCache.GetTypesDerivedFrom(attribute.TargetType))
            {
                string entryName = type.GetTypeName();
                menu.AddItem(entryName, () => AssignField(property, entryName));
            }

            menu.ShowMenu(buttonPosition);
        }

        private static void AssignField(SerializedProperty property, string newValue)
        {
            property.serializedObject.Update();
            property.stringValue = newValue;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
