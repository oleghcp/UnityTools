using System;
using UnityEditor;
using UnityEngine;
using OlegHcp;
using OlegHcp.Collections;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Window;

namespace OlegHcpEditor.Drawers.Attributes
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
            const float smallButtonWidth = 25f;

            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty arrayProp = property.FindPropertyRelative(BitList.ArrayFieldName);
            SerializedProperty lengthProp = property.FindPropertyRelative(BitList.LengthFieldName);
            SerializedProperty mutableProp = property.FindPropertyRelative(BitList.MutableFieldName);

            var data = EnumDropDownData.GetData(attribute.EnumType);
            string[] names = data.IndexableEnumNames;

            int intBlocksCount = BitList.GetArraySize(names.Length);
            arrayProp.arraySize = intBlocksCount;
            lengthProp.intValue = names.Length;

            Span<int> intBlocks = stackalloc int[intBlocksCount];
            intBlocks.Fill(i => arrayProp.GetArrayElementAtIndex(i).intValue);
            BitList bits = new BitList(intBlocks) { Length = names.Length };
            string buttonText = GetDropdownButtonText(bits, names);

            position.width -= smallButtonWidth;
            if (EditorGUI.DropdownButton(position, EditorGuiUtility.TempContent(buttonText), FocusType.Keyboard))
                EditorUtilityExt.DisplayMultiSelectableList(position, bits, names, onCloseMenu);

            position.xMin += position.width;
            position.width = smallButtonWidth;
            mutableProp.boolValue = EditorGui.ToggleButton(position, EditorGuiUtility.TempContent("M", "Switch mutability."), mutableProp.boolValue);

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

            if (bits.GetFlagsCount() == 1)
            {
                int index = first();
                if (index >= 0)
                    return displayedOptions[index];
            }

            return "Mixed...";

            bool none()
            {
                for (int i = 0; i < bits.Length; i++)
                {
                    if (displayedOptions[i].HasAnyData() && bits[i])
                        return false;
                }

                return true;
            }

            bool all()
            {
                for (int i = 0; i < bits.Length; i++)
                {
                    if (displayedOptions[i].HasAnyData() && !bits[i])
                        return false;
                }

                return true;
            }

            int first()
            {
                for (int i = 0; i < bits.Length; i++)
                {
                    if (displayedOptions[i].HasAnyData() && bits[i])
                        return i;
                }

                return -1;
            }
        }
    }
}
