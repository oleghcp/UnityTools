using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtility;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Drawers
{
    //Based on https://forum.unity.com/threads/serializereference-genericserializedreferenceinspectorui.813366/
    [CustomPropertyDrawer(typeof(ReferenceSelectionAttribute))]
    internal class ReferenceSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(fieldInfo).IsValueType)
            {
                EditorGui.DrawWrongTypeMessage(position, label, $"Use {nameof(ReferenceSelectionAttribute)} only with reference types.");
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            DrawSelectionButton(position, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        private void DrawSelectionButton(Rect position, SerializedProperty property)
        {
            float shift = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;

            Rect buttonPosition = position;
            buttonPosition.x += shift;
            buttonPosition.width = position.width - shift;
            buttonPosition.height = EditorGUIUtility.singleLineHeight;

            int storedIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            bool isNull = property.ManagedReferenceValueIsNull();

            string shortName;
            string toolTip;

            if (isNull)
            {
                shortName = "Null";
                toolTip = "Not assigned";
            }
            else
            {
                var (assemblyName, className) = EditorUtilityExt.SplitSerializedPropertyTypename(property.managedReferenceFullTypename);
                shortName = PathUtility.GetName(className, '.');
                toolTip = $"{className}  ({assemblyName})";
            }

            if (isNull)
                GUI.color = Colours.Cyan;

            if (GUI.Button(buttonPosition, new GUIContent(shortName, toolTip)))
                ShowContextMenu(property);

            GUI.color = Colours.White;
            EditorGUI.indentLevel = storedIndent;
        }

        private static void ShowContextMenu(SerializedProperty property)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Null"), false, () => AssignField(property, null));

            Type fieldType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFieldTypename);

            if (fieldType == null)
            {
                Debug.LogError("Can not get type from typename.");
                return;
            }

            addMenuItem(fieldType);

            foreach (Type type in TypeCache.GetTypesDerivedFrom(fieldType))
            {
                addMenuItem(type);
            }

            menu.ShowAsContext();

            void addMenuItem(Type type)
            {
                if (!type.IsAbstract && !type.IsInterface)
                {
                    string assemblyName = type.Assembly.ToString().Split('(', ',')[0];
                    string entryName = $"{type}  ({assemblyName})";
                    menu.AddItem(new GUIContent(entryName), false, () => AssignField(property, Activator.CreateInstance(type)));
                }
            }
        }

        private static void AssignField(SerializedProperty property, object newValue)
        {
            property.serializedObject.Update();
            property.managedReferenceValue = newValue;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
