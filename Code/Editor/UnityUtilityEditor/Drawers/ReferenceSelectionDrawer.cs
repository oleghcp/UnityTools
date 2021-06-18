using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtilityEditor.Window;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Drawers
{
    //Based on https://forum.unity.com/threads/serializereference-genericserializedreferenceinspectorui.813366/
    [CustomPropertyDrawer(typeof(ReferenceSelectionAttribute))]
    internal class ReferenceSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).IsValueType)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(ReferenceSelectionAttribute)} only with reference types.");
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            DrawSelectionButton(position, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        private void DrawSelectionButton(Rect position, SerializedProperty property)
        {
            float shift = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;

            position.x += shift;
            position.width -= shift;
            position.height = EditorGUIUtility.singleLineHeight;

            string typeName = property.managedReferenceFullTypename;
            bool nullRef = typeName.IsNullOrEmpty();
            string label = nullRef ? "Select Type" : typeName;

            if (nullRef)
                GUI.color = Colours.Orange;

            if (EditorGUI.DropdownButton(position, EditorGuiUtility.TempContent(label), FocusType.Keyboard))
                ShowContextMenu(position, property);

            GUI.color = Colours.White;
        }

        private static void ShowContextMenu(in Rect buttonPosition, SerializedProperty property)
        {
            Type fieldType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFieldTypename);

            if (fieldType == null)
            {
                Debug.LogError("Can not get type from typename.");
                return;
            }

            Type assignedType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFullTypename);
            var types = TypeCache.GetTypesDerivedFrom(fieldType);

            DropDownWindow menu = ScriptableObject.CreateInstance<DropDownWindow>();

            menu.AddItem("Null", assignedType == null, () => assignField(property, null));
            addMenuItem(fieldType);

            foreach (Type type in types)
            {
                addMenuItem(type);
            }

            menu.ShowMenu(buttonPosition);

            void addMenuItem(Type type)
            {
                if (isValidType(type))
                {
                    string assemblyName = type.Assembly.ToString().Split('(', ',')[0];
                    string entryName = $"{type}  ({assemblyName})";
                    menu.AddItem(entryName, type == assignedType, () => assignField(property, Activator.CreateInstance(type)));
                }
            }

            bool isValidType(Type type)
            {
                return !type.IsAbstract &&
                       !type.IsInterface &&
                       type.IsDefined(typeof(SerializableAttribute), false);
            }

            void assignField(SerializedProperty property, object newValue)
            {
                property.serializedObject.Update();
                property.managedReferenceValue = newValue;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
