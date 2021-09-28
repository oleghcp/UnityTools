using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtility.MathExt;
using UnityUtilityEditor.Window;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class EditorGui
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityObject[] DropArea(in Rect position)
        {
            return DropArea(position, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityObject[] DropArea(in Rect position, string text)
        {
            return DropArea(position, text, EditorStylesExt.DropArea);
        }

        public static UnityObject[] DropArea(in Rect position, string text, GUIStyle style)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ErrorLabel(in Rect position, GUIContent label, string message)
        {
            EditorGUI.LabelField(position, label, EditorGuiUtility.TempContent(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DropDown(in Rect propertyRect, int selectedIndex, string[] displayedOptions)
        {
            return DropDown(propertyRect, null, selectedIndex, displayedOptions);
        }

        public static int DropDown(Rect propertyRect, string label, int selectedIndex, string[] displayedOptions)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);

            if (label != null)
                propertyRect = EditorGUI.PrefixLabel(propertyRect, EditorGuiUtility.TempContent(label));

            selectedIndex = DropDownData.GetSelectedValueById(selectedIndex, controlId)
                                        .Clamp(0, displayedOptions.Length);

            if (EditorGUI.DropdownButton(propertyRect, EditorGuiUtility.TempContent(displayedOptions[selectedIndex]), FocusType.Keyboard))
            {
                EditorUtilityExt.DisplayDropDownList(propertyRect,
                                                     displayedOptions,
                                                     index => index == selectedIndex,
                                                     index => DropDownData.MenuAction(controlId, index));
            }

            return selectedIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IntDropDown(in Rect propertyRect, int selectedValue, string[] displayedOptions, int[] optionValues)
        {
            return IntDropDown(propertyRect, null, selectedValue, displayedOptions, optionValues);
        }

        public static int IntDropDown(in Rect propertyRect, string label, int selectedValue, string[] displayedOptions, int[] optionValues)
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
        public static Enum EnumDropDown(in Rect propertyRect, Enum selected)
        {
            return EnumDropDown(propertyRect, null, selected);
        }

        public static Enum EnumDropDown(in Rect propertyRect, string label, Enum selected)
        {
            var enumData = EnumDropDownData.GetData(selected.GetType());
            int index = Array.IndexOf(enumData.EnumValues, selected);
            int newIndex = DropDown(propertyRect, label, index, enumData.EnumNames);

            if (index != newIndex)
            {
                GUI.changed = true;
                return (Enum)enumData.EnumValues.GetValue(newIndex);
            }

            return selected;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaskDropDown(in Rect propertyRect, int mask, string[] displayedOptions, bool drawEmptyDisplayedOptions = false)
        {
            return MaskDropDown(propertyRect, null, mask, displayedOptions, drawEmptyDisplayedOptions);
        }

        public static int MaskDropDown(Rect propertyRect, string label, int mask, string[] displayedOptions, bool drawEmptyDisplayedOptions = false)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);

            if (label != null)
                propertyRect = EditorGUI.PrefixLabel(propertyRect, EditorGuiUtility.TempContent(label));

            int bitsCount = Math.Min(displayedOptions.Length, BitMask.SIZE);
            mask = DropDownData.GetSelectedValueById(mask, controlId);

            if (EditorGUI.DropdownButton(propertyRect, EditorGuiUtility.TempContent(getContentText()), FocusType.Keyboard))
            {
                EditorUtilityExt.DisplayMultiSelectableList(propertyRect,
                                                            BitList.CreateFromBitMask(mask, bitsCount),
                                                            displayedOptions,
                                                            drawEmptyDisplayedOptions,
                                                            bitList => DropDownData.MenuAction(controlId, bitList.ToIntBitMask()));
            }

            return mask;

            string getContentText()
            {
                if (BitMask.AllFor(mask, bitsCount))
                    return DropDownWindow.EVERYTHING_ITEM;

                if (BitMask.EmptyFor(mask, bitsCount))
                    return DropDownWindow.NOTHING_ITEM;

                return BitMask.EnumerateIndices(mask, bitsCount)
                              .Select(item => displayedOptions[item])
                              .ConcatToString(", ");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Enum FlagsDropDown(in Rect propertyRect, Enum flags)
        {
            return FlagsDropDown(propertyRect, null, flags);
        }

        public static Enum FlagsDropDown(in Rect propertyRect, string label, Enum flags)
        {
            Type type = flags.GetType();
            var enumData = EnumDropDownData.GetData(type);

            int mask = Convert.ToInt32(flags);
            int newMask = MaskDropDown(propertyRect, label, mask, enumData.EnumNames);

            if (mask != newMask)
            {
                GUI.changed = true;
                return (Enum)Enum.ToObject(type, newMask);
            }

            return flags;
        }
    }
}
