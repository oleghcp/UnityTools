using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Code.Runtime.UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    //Based on https://forum.unity.com/threads/serializereference-genericserializedreferenceinspectorui.813366/
    [CustomPropertyDrawer(typeof(TypeNameAttribute))]
    public class TypeNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType.GetTypeCode() != TypeCode.String)
            {
                EditorScriptUtility.DrawWrongTypeMessage(position, label, "Use TypeName only with strings.");
                return;
            }

            if (GetAttribute().TargetType.IsValueType)
            {
                EditorScriptUtility.DrawWrongTypeMessage(position, label, "Base type cannot be value type.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = GetAttribute().TargetType.GetTypeName();

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

            string shortName = savedType.IsAbstract ? $"Type name of {savedType.Name} (Abstract)" : "Type name of " + savedType.Name;
            string toolTip = savedType.GetTypeName();

            if (GetAttribute().TargetType == savedType)
                GUI.color = Colours.Yellow;

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

                foreach (Type type in TypeCache.GetTypesDerivedFrom(GetAttribute().TargetType))
                {
                    string entryName = type.GetTypeName();
                    context.AddItem(new GUIContent(entryName), false, initByInstance, entryName);
                }

                void initByBase()
                {
                    AssignField(property, GetAttribute().TargetType.GetTypeName());
                }

                void initByInstance(object typeName)
                {
                    AssignField(property, (string)typeName);
                }
            }
        }

        private static void AssignField(SerializedProperty property, string newValue)
        {
            property.serializedObject.Update();
            property.stringValue = newValue;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo(); // undo is bugged
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TypeNameAttribute GetAttribute()
        {
            return attribute as TypeNameAttribute;
        }
    }
}
