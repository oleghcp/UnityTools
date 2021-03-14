﻿using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
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
            else if (savedType.IsAbstract)
            {
                shortName = $"Typename of {savedType.Name} (Abstract)";
                GUI.color = Colours.Grey;
            }
            else
            {
                shortName = "Typename of " + savedType.Name;
            }

            if (EditorGUI.DropdownButton(position, EditorGuiUtility.TempContent(shortName), FocusType.Keyboard))
                ShowContextMenu(position, property);

            GUI.color = Colours.White;
        }

        private void ShowContextMenu(in Rect buttonPosition, SerializedProperty property)
        {
            DropDownList menu = DropDownList.Create();

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

            void assignField(SerializedProperty property, string newValue)
            {
                property.serializedObject.Update();
                property.stringValue = newValue;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
