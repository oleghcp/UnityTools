using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(InitListAttribute))]
    internal class InitListDrawer : SerializeReferenceDrawer
    {
        private FieldInfo[] _enumValues;

        protected override void DrawContent(in Rect position, SerializedProperty property)
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
                if (!property.HasManagedReferenceValue())
                    return null;

                Type bindedType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFullTypename);

                for (int i = 0; i < _enumValues.Length; i++)
                {
                    object enumValue = _enumValues[i].GetValue(null);

                    if (GetBindedType(enumValue) == bindedType)
                        return enumValue;
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
                Type newType = GetBindedType(newEnumValue);

                if (invalidType(newType, out string error))
                {
                    Debug.LogWarning(error);

                    Type oldType = GetBindedType(oldEnumValue);

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
                if (type == null)
                {
                    error = "Selecte enum value has no binded class type.";
                    return true;
                }

                if (type.IsValueType || !type.IsAssignableTo(EditorUtilityExt.GetFieldType(this)))
                {
                    error = $"Binded type ({type.Name}) is not subclass.";
                    return true;
                }

                if (type.IsAbstract || type.IsInterface)
                {
                    error = $"Binded type ({type.Name}) is abstract.";
                    return true;
                }

                error = null;
                return false;
            }
        }

        private Type GetBindedType(object enumValue)
        {
            if (enumValue == null)
                return null;

            long value = Convert.ToInt64(enumValue);

            FieldInfo fieldInfo = _enumValues.FirstOrDefault(compare);
            var attribute = fieldInfo?.GetCustomAttribute<BindSubclassAttribute>();
            return attribute?.ClassType;

            bool compare(FieldInfo fieldInfo)
            {
                object itemValue = fieldInfo.GetValue(null);
                long intValue = Convert.ToInt64(itemValue);
                return value == intValue;
            }
        }
    }
}
