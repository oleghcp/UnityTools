using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    //Based on https://forum.unity.com/threads/serializereference-genericserializedreferenceinspectorui.813366/
    [CustomPropertyDrawer(typeof(TypeNameAttribute))]
    public class TypeNameDrawer : AttributeDrawer<TypeNameAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(fieldInfo).GetTypeCode() != TypeCode.String)
            {
                GUIExt.DrawWrongTypeMessage(position, label, $"Use {nameof(TypeNameAttribute)} only with strings.");
                return;
            }

            if (property.stringValue.IsNullOrWhiteSpace())
                property.stringValue = attribute.TargetType.GetTypeName();

            EditorGUI.LabelField(position, label);
            DrawSelectionButton(position, property);
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

            if (GUI.Button(buttonPosition, new GUIContent(shortName, toolTip)))
                f_showContextMenu(property);

            GUI.color = Colours.White;
            EditorGUI.indentLevel = storedIndent;
        }

        private void f_showContextMenu(SerializedProperty property)
        {
            GenericMenu context = new GenericMenu();

            context.AddItem(new GUIContent("Base"), false, initByBase);

            foreach (Type type in TypeCache.GetTypesDerivedFrom(attribute.TargetType))
            {
                string entryName = type.GetTypeName();
                context.AddItem(new GUIContent(entryName), false, initByInstance, entryName);
            }

            context.ShowAsContext();

            void initByBase()
            {
                AssignField(property, attribute.TargetType.GetTypeName());
            }

            void initByInstance(object typeName)
            {
                AssignField(property, (string)typeName);
            }
        }

        private static void AssignField(SerializedProperty property, string newValue)
        {
            property.serializedObject.Update();
            property.stringValue = newValue;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
