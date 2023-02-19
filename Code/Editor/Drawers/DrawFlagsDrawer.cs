using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtility.CSharp;
using UnityUtility.Inspector;
using UnityUtilityEditor.Engine;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawFlagsAttribute))]
    internal class DrawFlagsDrawer : AttributeDrawer<DrawFlagsAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            if (type == typeof(int))
            {
                DrawIntMask(position, property, label, true);
                return;
            }

            if (type == typeof(IntMask))
            {
                DrawIntMask(position, property, label, false);
                return;
            }

            if (type == typeof(BitList))
            {
                DrawBitList(position, property, label);
                return;
            }

            EditorGui.ErrorLabel(position, label, $"Use {nameof(DrawFlagsAttribute)} with {nameof(Int32)}, {nameof(IntMask)} or {nameof(BitList)}.");
        }

        private void DrawIntMask(in Rect position, SerializedProperty property, GUIContent label, bool int32)
        {
            var data = EnumDropDownData.GetData(attribute.EnumType);
            string[] names = data.IndexableEnumNames;

            if (names.Length > BitMask.SIZE)
            {
                Debug.LogError($"Enum values are out of range. Max enum value must be less than {BitMask.SIZE}.");
                return;
            }

            if (int32)
                property.intValue = EditorGui.MaskDropDown(position, label, property.intValue, names);
            else
                property.SetIntMaskValue(EditorGui.MaskDropDown(position, label, (int)property.GetIntMaskValue(), names));
        }

        private void DrawBitList(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty arrayProp = property.FindPropertyRelative(BitList.ArrayFieldName);
            SerializedProperty lengthProp = property.FindPropertyRelative(BitList.LengthFieldName);

            var data = EnumDropDownData.GetData(attribute.EnumType);
            string[] names = data.IndexableEnumNames;

            int intBlocksCount = BitList.GetArraySize(names.Length);
            arrayProp.arraySize = intBlocksCount;
            lengthProp.intValue = names.Length;

            Span<int> intBlocks = stackalloc int[intBlocksCount];
            intBlocks.Fill(i => arrayProp.GetArrayElementAtIndex(i).intValue);
            BitList bits = new BitList(intBlocks) { Count = names.Length };
            string buttonText = GetDropdownButtonText(bits, names);

            if (EditorGUI.DropdownButton(position, EditorGuiUtility.TempContent(buttonText), FocusType.Keyboard))
                EditorUtilityExt.DisplayMultiSelectableList(position, bits, names, onCloseMenu);

            void onCloseMenu(BitList bitList)
            {
                if (property.serializedObject.Disposed())
                    return;

                property.serializedObject.Update();
                for (int i = 0; i < bitList.IntBlocks.Count; i++)
                {
                    arrayProp.GetArrayElementAtIndex(i).intValue = bitList.IntBlocks[i];
                }
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private static string GetDropdownButtonText(BitList bits, string[] displayedOptions)
        {
            if (none())
                return DropDownWindow.NOTHING_ITEM;

            if (all())
                return DropDownWindow.EVERYTHING_ITEM;

            if (bits.GetCount() == 1)
            {
                int index = first();
                if (index >= 0)
                    return displayedOptions[index];
            }

            return "Mixed...";

            bool none()
            {
                for (int i = 0; i < bits.Count; i++)
                {
                    if (displayedOptions[i].HasAnyData() && bits[i])
                        return false;
                }

                return true;
            }

            bool all()
            {
                for (int i = 0; i < bits.Count; i++)
                {
                    if (displayedOptions[i].HasAnyData() && !bits[i])
                        return false;
                }

                return true;
            }

            int first()
            {
                for (int i = 0; i < bits.Count; i++)
                {
                    if (displayedOptions[i].HasAnyData() && bits[i])
                        return i;
                }

                return -1;
            }
        }
    }
}
