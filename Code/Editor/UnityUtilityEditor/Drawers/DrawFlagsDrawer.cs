using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtility.Inspector;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawFlagsAttribute))]
    internal class DrawFlagsDrawer : AttributeDrawer<DrawFlagsAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FieldType fieldType = GetFieldType();

            if (fieldType == FieldType.Other)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(DrawFlagsAttribute)} with {nameof(Int32)}, {nameof(IntMask)} or {nameof(BitList)}.");
                return;
            }

            position = EditorGUI.PrefixLabel(position, label);

            if (EditorGUI.DropdownButton(position, EditorGuiUtility.TempContent("Edit values"), FocusType.Keyboard))
            {
                switch (fieldType)
                {
                    case FieldType.Int32:
                        ShowIntMaskMenu(position, property, true);
                        break;

                    case FieldType.IntMask:
                        ShowIntMaskMenu(position, property, false);
                        break;

                    case FieldType.BitList:
                        ShowBitListMenu(position, property);
                        break;

                    default:
                        throw new UnsupportedValueException(fieldType);
                }
            }
        }

        private FieldType GetFieldType()
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            if (type.GetTypeCode() == TypeCode.Int32)
                return FieldType.Int32;

            if (type == typeof(IntMask))
                return FieldType.IntMask;

            if (type == typeof(BitList))
                return FieldType.BitList;

            return FieldType.Other;
        }

        private void ShowIntMaskMenu(in Rect position, SerializedProperty property, bool int32)
        {
            var data = EnumDropDownData.GetData(attribute.EnumType);
            Array enumValues = data.EnumValues;

            int length = 0;
            if (enumValues.Length > 0)
            {
                Enum lastElement = enumValues.GetValue(enumValues.Length - 1) as Enum;
                length = Convert.ToInt32(lastElement) + 1;
            }

            if (length > BitMask.SIZE)
            {
                Debug.LogError($"Enum values are out of range. Max enum value must be less than {BitMask.SIZE}.");
                return;
            }

            string[] names = new string[length];

            foreach (Enum item in enumValues)
            {
                names[Convert.ToInt32(item)] = item.GetName();
            }

            int mask = int32 ? property.intValue : (int)property.GetIntMaskValue();
            BitList bits = BitList.CreateFromBitMask(mask, names.Length);
            EditorUtilityExt.DisplayMultiSelectableList(position, bits, names, onCloseMenu);

            void onCloseMenu(BitList bitList)
            {
                if (property.serializedObject.Disposed())
                    return;

                property.serializedObject.Update();
                if (int32)
                    property.intValue = bitList.ToIntBitMask();
                else
                    property.SetIntMaskValue(bitList.ToIntBitMask());
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void ShowBitListMenu(in Rect position, SerializedProperty property)
        {
            var data = EnumDropDownData.GetData(attribute.EnumType);
            Array enumValues = data.EnumValues;

            int length = 0;
            if (enumValues.Length > 0)
            {
                Enum lastElement = enumValues.GetValue(enumValues.Length - 1) as Enum;
                length = Convert.ToInt32(lastElement) + 1;
            }

            string[] names = new string[length];

            foreach (Enum item in enumValues)
            {
                names[Convert.ToInt32(item)] = item.GetName();
            }

            SerializedProperty arrayProp = property.FindPropertyRelative(BitList.ArrayFieldName);
            SerializedProperty lengthProp = property.FindPropertyRelative(BitList.LengthFieldName);

            int intBlocksCount = BitList.GetArraySize(names.Length);
            arrayProp.arraySize = intBlocksCount;
            lengthProp.intValue = names.Length;

            Span<int> intBlocks = stackalloc int[intBlocksCount];
            for (int i = 0; i < intBlocks.Length; i++)
            {
                intBlocks[i] = arrayProp.GetArrayElementAtIndex(i).intValue;
            }
            BitList bits = new BitList(intBlocks)
            {
                Count = names.Length
            };

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

        private enum FieldType
        {
            Other,
            Int32,
            IntMask,
            BitList,
        }
    }
}
