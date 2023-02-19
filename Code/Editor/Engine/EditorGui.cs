using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtility.CSharp;
using UnityUtility.Mathematics;
using UnityUtility.NumericEntities;
using UnityUtilityEditor.Drawers;
using UnityUtilityEditor.Window;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Engine
{
    public static class EditorGui
    {
        internal static void ErrorLabel(in Rect position, GUIContent label, string message)
        {
            EditorGUI.LabelField(position, label, EditorGuiUtility.TempContent(message));
        }

        public static Diapason DiapasonField(in Rect position, string text, Diapason diapason, float minLimit = float.NegativeInfinity, float maxLimit = float.PositiveInfinity)
        {
            return DiapasonField(position, EditorGuiUtility.TempContent(text), diapason, minLimit, maxLimit);
        }

        public static Diapason DiapasonField(in Rect position, GUIContent label, Diapason diapason, float minLimit = float.NegativeInfinity, float maxLimit = float.PositiveInfinity)
        {
            Rect rect = EditorGUI.PrefixLabel(position, label);

            float[] array = { diapason.Min, diapason.Max };

            EditorGUI.MultiFloatField(rect, DiapasonDrawerHelper.SubLabels, array);

            diapason.Min = array[0] = array[0].Clamp(minLimit, maxLimit);
            diapason.Max = array[1].Clamp(array[0], maxLimit);

            return diapason;
        }

        public static DiapasonInt DiapasonIntField(in Rect position, string text, DiapasonInt diapason, int minLimit = int.MinValue, int maxLimit = int.MaxValue)
        {
            return DiapasonIntField(position, EditorGuiUtility.TempContent(text), diapason, minLimit, maxLimit);
        }

        public static DiapasonInt DiapasonIntField(in Rect position, GUIContent label, DiapasonInt diapason, int minLimit = int.MinValue, int maxLimit = int.MaxValue)
        {
            Rect rect = EditorGUI.PrefixLabel(position, label);

            int[] array = { diapason.Min, diapason.Max };

            EditorGUI.MultiIntField(rect, DiapasonDrawerHelper.SubLabels, array);

            diapason.Min = array[0] = array[0].Clamp(minLimit, maxLimit);
            diapason.Max = array[1].Clamp(array[0], maxLimit);

            return diapason;
        }

        public static bool ToggleButton(in Rect position, string text, bool value)
        {
            return ToggleButton(position, text, value, GUI.skin.button);
        }

        public static bool ToggleButton(in Rect position, string text, bool value, GUIStyle style)
        {
            return GUI.Toggle(position, value, text, style);
        }

        public static bool ToggleButton(in Rect position, GUIContent content, bool value)
        {
            return ToggleButton(position, content, value, GUI.skin.button);
        }

        public static bool ToggleButton(in Rect position, GUIContent content, bool value, GUIStyle style)
        {
            return GUI.Toggle(position, value, content, style);
        }

        public static UnityObject[] DropArea(in Rect position)
        {
            return DropArea(position, GUIContent.none);
        }

        public static UnityObject[] DropArea(in Rect position, string text)
        {
            return DropArea(position, text, EditorStylesExt.DropArea);
        }

        public static UnityObject[] DropArea(in Rect position, GUIContent content)
        {
            return DropArea(position, content, EditorStylesExt.DropArea);
        }

        public static UnityObject[] DropArea(in Rect position, string text, GUIStyle style)
        {
            return DropArea(position, EditorGuiUtility.TempContent(text), style);
        }

        public static UnityObject[] DropArea(in Rect position, GUIContent content, GUIStyle style)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);
            UnityObject[] objects = DragAndDropData.GetObjectsById(controlId);

            GUI.Box(position, content, style);

            if (EditorUtilityExt.GetDroppedObjects(position, out var result))
            {
                DragAndDropData.PlaceObjects(controlId, result);
                GUI.changed = true;
            }

            return objects;
        }

        public static int DropDown(in Rect propertyRect, int selectedIndex, string[] displayedOptions)
        {
            return DropDown(propertyRect, GUIContent.none, selectedIndex, displayedOptions);
        }

        public static int DropDown(in Rect propertyRect, string label, int selectedIndex, string[] displayedOptions)
        {
            return DropDown(propertyRect, EditorGuiUtility.TempContent(label), selectedIndex, displayedOptions);
        }

        public static int DropDown(Rect propertyRect, GUIContent label, int selectedIndex, string[] displayedOptions)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);

            propertyRect = EditorGUI.PrefixLabel(propertyRect, label);

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

        public static int IntDropDown(in Rect propertyRect, int selectedValue, string[] displayedOptions, int[] optionValues)
        {
            return IntDropDown(propertyRect, GUIContent.none, selectedValue, displayedOptions, optionValues);
        }

        public static int IntDropDown(in Rect propertyRect, string label, int selectedValue, string[] displayedOptions, int[] optionValues)
        {
            return IntDropDown(propertyRect, EditorGuiUtility.TempContent(label), selectedValue, displayedOptions, optionValues);
        }

        public static int IntDropDown(in Rect propertyRect, GUIContent label, int selectedValue, string[] displayedOptions, int[] optionValues)
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

        public static Enum EnumDropDown(in Rect propertyRect, Enum selected)
        {
            return EnumDropDown(propertyRect, GUIContent.none, selected);
        }

        public static Enum EnumDropDown(in Rect propertyRect, string label, Enum selected)
        {
            return EnumDropDown(propertyRect, EditorGuiUtility.TempContent(label), selected);
        }

        public static Enum EnumDropDown(in Rect propertyRect, GUIContent label, Enum selected)
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

        public static int MaskDropDown(in Rect propertyRect, int mask, string[] displayedOptions)
        {
            return MaskDropDown(propertyRect, GUIContent.none, mask, displayedOptions);
        }

        public static int MaskDropDown(in Rect propertyRect, string label, int mask, string[] displayedOptions)
        {
            return MaskDropDown(propertyRect, EditorGuiUtility.TempContent(label), mask, displayedOptions);
        }

        public static int MaskDropDown(Rect propertyRect, GUIContent label, int mask, string[] displayedOptions)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);

            propertyRect = EditorGUI.PrefixLabel(propertyRect, label);

            int flagsCount = Math.Min(displayedOptions.Length, BitMask.SIZE);
            mask = DropDownData.GetSelectedValueById(mask, controlId);

            string buttonText = GetDropdownButtonText(mask, flagsCount, displayedOptions);
            if (EditorGUI.DropdownButton(propertyRect, EditorGuiUtility.TempContent(buttonText), FocusType.Keyboard))
            {
                EditorUtilityExt.DisplayMultiSelectableList(propertyRect,
                                                            BitList.CreateFromBitMask(mask, flagsCount),
                                                            displayedOptions,
                                                            bitList => DropDownData.MenuAction(controlId, bitList.ToIntBitMask()));
            }

            return mask;
        }

        private static string GetDropdownButtonText(int mask, int bitsCount, string[] displayedOptions)
        {
            if (none())
                return DropDownWindow.NOTHING_ITEM;

            if (all())
                return DropDownWindow.EVERYTHING_ITEM;

            if (BitMask.GetCount(mask, bitsCount) == 1)
            {
                int index = first();
                if (index >= 0)
                    return displayedOptions[index];
            }

            return "Mixed...";

            bool none()
            {
                for (int i = 0; i < bitsCount; i++)
                {
                    if (displayedOptions[i].HasAnyData() && BitMask.HasFlag(mask, i))
                        return false;
                }

                return true;
            }

            bool all()
            {
                for (int i = 0; i < bitsCount; i++)
                {
                    if (displayedOptions[i].HasAnyData() && !BitMask.HasFlag(mask, i))
                        return false;
                }

                return true;
            }

            int first()
            {
                for (int i = 0; i < bitsCount; i++)
                {
                    if (displayedOptions[i].HasAnyData() && BitMask.HasFlag(mask, i))
                        return i;
                }

                return -1;
            }
        }

        public static Enum FlagsDropDown(in Rect propertyRect, Enum flags)
        {
            return FlagsDropDown(propertyRect, GUIContent.none, flags);
        }

        public static Enum FlagsDropDown(in Rect propertyRect, string label, Enum flags)
        {
            return FlagsDropDown(propertyRect, EditorGuiUtility.TempContent(label), flags);
        }

        public static Enum FlagsDropDown(in Rect propertyRect, GUIContent label, Enum flags)
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
