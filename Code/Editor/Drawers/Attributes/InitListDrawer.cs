using System;
using System.Linq;
using System.Reflection;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(InitListAttribute))]
    internal class InitListDrawer : SerializeReferenceDrawer
    {
        private FieldInfo[] _enumValues;

        protected override void DrawExtendedContent(in Rect position, SerializedProperty property)
        {
            InitListAttribute a = attribute as InitListAttribute;

            if (!a.EnumType.IsEnum)
            {
                GUI.Label(position, "Specify enum type.");
                return;
            }

            if (_enumValues == null)
                _enumValues = a.EnumType.GetFields(BindingFlags.Static | BindingFlags.Public);

            object enumValue = getEnumValue();

            if (enumValue == null)
            {
                enumValue = a.EnumType.GetDefaultValue();
                enumValue = Enum.ToObject(a.EnumType, enumValue);
                InitFiled(property, enumValue);
            }

            int level = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            Enum newEnumValue = EditorGUI.EnumPopup(position, (Enum)enumValue);
            EditorGUI.indentLevel = level;

            if (!enumValue.Equals(newEnumValue))
                InitFiled(property, newEnumValue, enumValue);

            object getEnumValue()
            {
                Type boundType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFullTypename);

                if (boundType == null)
                    return null;

                for (int i = 0; i < _enumValues.Length; i++)
                {
                    object value = _enumValues[i].GetValue(null);

                    if (GetBoundType(value) == boundType)
                        return value;
                }

                return null;
            }
        }

        private void InitFiled(SerializedProperty property, object newEnumValue, object oldEnumValue = null)
        {
            property.serializedObject.Update();
            property.managedReferenceValue = getInstance();
            property.isExpanded = false;
            property.serializedObject.ApplyModifiedProperties();

            object getInstance()
            {
                Type newType = GetBoundType(newEnumValue);

                if (newType == null)
                    return null;

                if (invalidType(newType, out string error))
                {
                    Debug.LogWarning(error);

                    Type oldType = GetBoundType(oldEnumValue);

                    if (invalidType(oldType, out error))
                    {
                        Debug.LogWarning(error);
                        return null;
                    }

                    return Activator.CreateInstance(oldType);
                }

                return Activator.CreateInstance(newType);
            }

            bool invalidType(Type type, out string error)
            {
                if (type.IsValueType || !type.IsAssignableTo(EditorUtilityExt.GetFieldType(this)))
                {
                    error = $"Bound type ({type.Name}) is not subclass.";
                    return true;
                }

                if (type.IsAbstract)
                {
                    error = $"Bound type ({type.Name}) is abstract.";
                    return true;
                }

                error = null;
                return false;
            }
        }

        private Type GetBoundType(object enumValue)
        {
            if (enumValue == null)
                return null;

            long value = Convert.ToInt64(enumValue);

            FieldInfo fieldInfo = _enumValues.FirstOrDefault(compare);
            var attribute = fieldInfo?.GetCustomAttribute<BindSubclassAttribute>();
            return attribute?.ClassType;

            bool compare(FieldInfo field)
            {
                object itemValue = field.GetValue(null);
                long intValue = Convert.ToInt64(itemValue);
                return value == intValue;
            }
        }
    }
}
