using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    //Based on https://forum.unity.com/threads/serializereference-genericserializedreferenceinspectorui.813366/
    [CustomPropertyDrawer(typeof(SerializeReferenceSelectionAttribute))]
    public class SerializeReferenceSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType.IsValueType)
            {
                EditorScriptUtility.DrawWrongTypeMessage(position, label, "Use SerializeReferenceSelectionAttribute only with reference types.");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            f_drawSelectionButton(position, property);
            EditorGUI.PropertyField(position, property, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        private static void f_drawSelectionButton(Rect position, SerializedProperty property)
        {
            float shift = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;

            Rect buttonPosition = position;
            buttonPosition.x += shift;
            buttonPosition.width = position.width - shift;
            buttonPosition.height = EditorGUIUtility.singleLineHeight;

            int storedIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            Color storedColor = GUI.backgroundColor;

            var (assemblyName, className) = EditorUtilityExt.SplitSerializedPropertyTypename(property.managedReferenceFullTypename);

            if (className.IsNullOrEmpty())
                className = "Null (Assign)";

            if (GUI.Button(buttonPosition, new GUIContent(className, $"{className}  ( {assemblyName} )")))
            {
                f_showContextMenu(property);
            }

            GUI.backgroundColor = storedColor;
            EditorGUI.indentLevel = storedIndent;
        }

        private static void f_showContextMenu(SerializedProperty property)
        {
            GenericMenu context = new GenericMenu();
            fillContextMenu();
            context.ShowAsContext();

            void fillContextMenu()
            {
                context.AddItem(new GUIContent("Null"), false, initByNull);

                Type fieldType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFieldTypename);

                if (fieldType == null)
                {
                    Debug.LogError("Can not get type from typename.");
                    return;
                }

                foreach (Type type in TypeCache.GetTypesDerivedFrom(fieldType))
                {
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    string assemblyName = type.Assembly.ToString().Split('(', ',')[0];
                    string entryName = $"{type}  ( {assemblyName} )";
                    context.AddItem(new GUIContent(entryName), false, initByInstance, type);
                }

                void initByNull()
                {
                    AssignField(property, null);
                }

                void initByInstance(object typeAsObject)
                {
                    AssignField(property, Activator.CreateInstance((Type)typeAsObject));
                }
            }
        }

        private static void AssignField(SerializedProperty property, object newValue)
        {
            property.serializedObject.Update();
            property.managedReferenceValue = newValue;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo(); // undo is bugged
        }
    }
}
