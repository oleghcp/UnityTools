using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    //Based on https://forum.unity.com/threads/serializereference-genericserializedreferenceinspectorui.813366/
    [CustomPropertyDrawer(typeof(TypeNameAttribute))]
    public class TypeNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorScriptUtility.GetFieldType(fieldInfo).GetTypeCode() != TypeCode.String)
            {
                EditorScriptUtility.DrawWrongTypeMessage(position, label, $"Use {nameof(TypeNameAttribute)} only with strings.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = f_GetAttribute().TargetType.GetTypeName();

            EditorGUI.LabelField(position, label);
            f_drawSelectionButton(position, property);
        }



        private void f_drawSelectionButton(Rect position, SerializedProperty property)
        {
            float shift = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;

            Rect buttonPosition = position;
            buttonPosition.x += shift;
            buttonPosition.width = position.width - shift;
            buttonPosition.height = EditorGUIUtility.singleLineHeight;

            int storedIndent = EditorGUI.indentLevel;
            Color storedColor = GUI.color;
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

                if (f_GetAttribute().TargetType == savedType)
                    GUI.color = Colours.Yellow;
            }

            if (GUI.Button(buttonPosition, new GUIContent(shortName, toolTip)))
                f_showContextMenu(property);

            GUI.color = storedColor;
            EditorGUI.indentLevel = storedIndent;
        }

        private void f_showContextMenu(SerializedProperty property)
        {
            GenericMenu context = new GenericMenu();
            fillContextMenu();
            context.ShowAsContext();

            void fillContextMenu()
            {
                context.AddItem(new GUIContent("Base"), false, initByBase);

                foreach (Type type in TypeCache.GetTypesDerivedFrom(f_GetAttribute().TargetType))
                {
                    string entryName = type.GetTypeName();
                    context.AddItem(new GUIContent(entryName), false, initByInstance, entryName);
                }

                void initByBase()
                {
                    f_assignField(property, f_GetAttribute().TargetType.GetTypeName());
                }

                void initByInstance(object typeName)
                {
                    f_assignField(property, (string)typeName);
                }
            }
        }

        private static void f_assignField(SerializedProperty property, string newValue)
        {
            property.serializedObject.Update();
            property.stringValue = newValue;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo(); // undo is bugged
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TypeNameAttribute f_GetAttribute()
        {
            return attribute as TypeNameAttribute;
        }
    }
}
