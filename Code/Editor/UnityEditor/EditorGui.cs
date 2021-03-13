using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.MathExt;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class EditorGui
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityObject[] DropArea(Rect position)
        {
            return DropArea(position, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityObject[] DropArea(Rect position, string text)
        {
            return DropArea(position, text, EditorStylesExt.DropArea);
        }

        public static UnityObject[] DropArea(Rect position, string text, GUIStyle style)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);
            UnityObject[] objects = DragAndDropData.GetObjectsById(controlId);

            GUI.Box(position, text, style);

            if (EditorUtilityExt.GetDroppedObjects(position, out var result))
            {
                DragAndDropData.PlaceObjects(controlId, result);
                GUI.changed = true;
            }

            return objects;
        }

        internal static void WrongTypeLabel(Rect position, GUIContent label, string message)
        {
            Rect rect = EditorGUI.PrefixLabel(position, label);
            GUI.Label(rect, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DropDown(Rect propertyRect, int selectedIndex, string[] displayedOptions)
        {
            return DropDown(propertyRect, null, selectedIndex, displayedOptions);
        }

        public static int DropDown(Rect propertyRect, string label, int selectedIndex, string[] displayedOptions)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);

            if (label != null)
                propertyRect = EditorGUI.PrefixLabel(propertyRect, new GUIContent(label));

            selectedIndex = DropDownData.GetSelectedIndexById(selectedIndex, controlId)
                                        .Clamp(0, displayedOptions.Length);

            if (EditorGUI.DropdownButton(propertyRect, new GUIContent(displayedOptions[selectedIndex]), FocusType.Keyboard))
            {
                EditorUtilityExt.DisplayDropDownList(propertyRect,
                                                     displayedOptions,
                                                     index => index == selectedIndex,
                                                     index => DropDownData.MenuAction(controlId, index));
            }

            return selectedIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IntDropDown(Rect propertyRect, int selectedValue, string[] displayedOptions, int[] optionValues)
        {
            return IntDropDown(propertyRect, null, selectedValue, displayedOptions, optionValues);
        }

        public static int IntDropDown(Rect propertyRect, string label, int selectedValue, string[] displayedOptions, int[] optionValues)
        {
            if (displayedOptions.Length != optionValues.Length)
            {
                Debug.LogError("Different array sizes.");
                return 0;
            }

            int index = optionValues.IndexOf(selectedValue);
            index = DropDown(propertyRect, label, index, displayedOptions);
            return optionValues[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Enum EnumDropDown(Rect propertyRect, Enum selected)
        {
            return EnumDropDown(propertyRect, null, selected);
        }

        public static Enum EnumDropDown(Rect propertyRect, string label, Enum selected)
        {
            var enumData = EnumDropDownData.GetData(selected.GetType());

            int index = Array.IndexOf(enumData.EnumValues, selected);
            index = DropDown(propertyRect, label, index, enumData.EnumNames);
            return (Enum)enumData.EnumValues.GetValue(index);
        }
    }
}
