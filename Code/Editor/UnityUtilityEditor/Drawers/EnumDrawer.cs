using System;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(Enum), true)]
    internal sealed class EnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (hasEnumFlagsAttribute())
                property.intValue = EditorGui.MaskDropDown(position, label.text, property.intValue, property.enumDisplayNames);
            else
                property.enumValueIndex = EditorGui.DropDown(position, label.text, property.enumValueIndex, property.enumDisplayNames);

            bool hasEnumFlagsAttribute()
            {
                Type fieldType = EditorUtilityExt.GetFieldType(this);
                return fieldType.IsDefined(typeof(FlagsAttribute), false);
            }
        }
    }
}