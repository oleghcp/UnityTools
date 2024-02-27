using System;
using UnityEditor;
using UnityEngine;
using OlegHcp;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Window;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(TypeNameAttribute))]
    internal class TypeNameDrawer : AttributeDrawer<TypeNameAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.String)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(TypeNameAttribute)} only with strings.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = attribute.TargetType.GetTypeName();

            position = EditorGUI.PrefixLabel(position, label);
            DrawSelectionButton(position, property);
        }

        private void DrawSelectionButton(in Rect position, SerializedProperty property)
        {
            Type savedType = Type.GetType(property.stringValue);

            string shortName;

            if (savedType == null)
            {
                shortName = "Unknown";
                GUI.color = Colours.Red;
            }
            else
            {
                if (savedType.IsAbstract)
                    GUI.color = Colours.Orange;

                shortName = savedType.Name;
            }

            if (EditorGUI.DropdownButton(position, EditorGuiUtility.TempContent(shortName), FocusType.Keyboard))
                ShowContextMenu(position, property);

            GUI.color = Colours.White;
        }

        private void ShowContextMenu(in Rect buttonPosition, SerializedProperty property)
        {
            DropDownWindow menu = ScriptableObject.CreateInstance<DropDownWindow>();

            addMenuItem(attribute.TargetType.GetTypeName());

            foreach (Type type in TypeCache.GetTypesDerivedFrom(attribute.TargetType))
            {
                addMenuItem(type.GetTypeName());
            }

            menu.ShowMenu(buttonPosition);

            void addMenuItem(string entryName)
            {
                menu.AddItem(entryName, entryName == property.stringValue, () => assignField(property, entryName));
            }

            void assignField(SerializedProperty prop, string newValue)
            {
                prop.serializedObject.Update();
                prop.stringValue = newValue;
                prop.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
