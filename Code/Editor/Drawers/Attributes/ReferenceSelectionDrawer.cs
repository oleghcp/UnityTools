using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.CSharp;
using UnityUtility.Inspector;
using UnityUtilityEditor.Engine;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor.Drawers.Attributes
{
    //Based on https://forum.unity.com/threads/serializereference-genericserializedreferenceinspectorui.813366/
    [CustomPropertyDrawer(typeof(ReferenceSelectionAttribute))]
    internal class ReferenceSelectionDrawer : SerializeReferenceDrawer
    {
        protected override void DrawExtendedContent(in Rect position, SerializedProperty property)
        {
            DrawContent(position, property);
        }

        public static void DrawContent(in Rect position, SerializedProperty property, params Type[] rootTypes)
        {
            Type assignedType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFullTypename);
            bool nullRef = assignedType == null;
            string label = nullRef ? "Null" : assignedType.Name;

            if (nullRef)
                GUI.color = Colours.Orange;

            if (EditorGUI.DropdownButton(position, EditorGuiUtility.TempContent(label), FocusType.Keyboard))
                ShowContextMenu(position, property, rootTypes);

            GUI.color = Colours.White;
        }

        private static void ShowContextMenu(in Rect buttonPosition, SerializedProperty property, params Type[] rootTypes)
        {
            Type fieldType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFieldTypename);

            if (fieldType == null)
            {
                Debug.LogError("Can not get type from typename.");
                return;
            }

            DropDownWindow menu = ScriptableObject.CreateInstance<DropDownWindow>();

            Type assignedType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFullTypename);
            menu.AddItem("Null", assignedType == null, () => assignField(property, null));

            foreach (Type rootType in rootTypes)
            {
                if (!rootType.IsAssignableTo(fieldType))
                {
                    Debug.LogWarning($"{rootType.Name} is not subclass of {fieldType.Name}");
                    continue;
                }

                addMenuItem(rootType);

                foreach (Type type in TypeCache.GetTypesDerivedFrom(rootType))
                {
                    addMenuItem(type);
                }
            }

            menu.ShowMenu(buttonPosition);

            void addMenuItem(Type type)
            {
                if (isValidType(type))
                {
                    string entryName = $"{type.Name}  ({type.Namespace})";
                    menu.AddItem(entryName, type == assignedType, () => assignField(property, Activator.CreateInstance(type)));
                }
            }

            bool isValidType(Type type)
            {
                return !type.IsAbstract && !type.IsInterface;
            }

            void assignField(SerializedProperty prop, object newValue)
            {
                prop.serializedObject.Update();
                prop.managedReferenceValue = newValue;
                prop.isExpanded = false;
                prop.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
