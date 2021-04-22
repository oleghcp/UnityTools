using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(EnumMenuAttribute), true)]
    internal sealed class EnumMenuDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type fieldType = EditorUtilityExt.GetFieldType(this);

            if (!fieldType.IsEnum)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(EnumMenuAttribute)} with enum.");
                return;
            }

            if (fieldType.IsDefined(typeof(FlagsAttribute), false))
                property.intValue = EditorGui.MaskDropDown(position, label.text, property.intValue, property.enumDisplayNames);
            else
                property.enumValueIndex = EditorGui.DropDown(position, label.text, property.enumValueIndex, property.enumDisplayNames);
        }
    }
}
