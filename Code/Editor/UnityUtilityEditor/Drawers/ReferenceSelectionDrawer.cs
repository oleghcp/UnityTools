﻿using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtility;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Drawers
{
    //Based on https://forum.unity.com/threads/serializereference-genericserializedreferenceinspectorui.813366/
    [CustomPropertyDrawer(typeof(ReferenceSelectionAttribute))]
    public class ReferenceSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(fieldInfo).IsValueType)
            {
                GUIExt.DrawWrongTypeMessage(position, label, $"Use {nameof(ReferenceSelectionAttribute)} only with reference types.");
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            f_drawSelectionButton(position, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        private void f_drawSelectionButton(Rect position, SerializedProperty property)
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
                f_showContextMenu(property);

            GUI.color = Colours.White;
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

                addMenuItem(fieldType);

                foreach (Type type in TypeCache.GetTypesDerivedFrom(fieldType))
                {
                    addMenuItem(type);
                }

                void addMenuItem(Type type)
                {
                    if (!type.IsAbstract && !type.IsInterface)
                    {
                        string assemblyName = type.Assembly.ToString().Split('(', ',')[0];
                        string entryName = $"{type}  ({assemblyName})";
                        context.AddItem(new GUIContent(entryName), false, initByInstance, type);
                    }
                }

                void initByNull()
                {
                    f_assignField(property, null);
                }

                void initByInstance(object typeAsObject)
                {
                    f_assignField(property, Activator.CreateInstance((Type)typeAsObject));
                }
            }
        }

        private static void f_assignField(SerializedProperty property, object newValue)
        {
            property.serializedObject.Update();
            property.managedReferenceValue = newValue;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
