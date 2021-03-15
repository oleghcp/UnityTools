using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawFlagsAttribute))]
    internal class DrawFlagsDrawer : AttributeDrawer<DrawFlagsAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorUtilityExt.GetFieldType(this);
            bool integerMask = type.GetTypeCode() == TypeCode.Int32;

            if (type != typeof(BitList) && !integerMask)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(DrawFlagsAttribute)} with {nameof(Int32)} or with {nameof(BitList)}.");
                return;
            }

            position = EditorGUI.PrefixLabel(position, label);

            if (EditorGUI.DropdownButton(position, EditorGuiUtility.TempContent("Edit values"), FocusType.Keyboard))
            {
                if (integerMask)
                    ShowIntMaskMenu(position, property);
                else
                    ShowBitListMenu(position, property);
            }
        }

        private void ShowIntMaskMenu(in Rect position, SerializedProperty property)
        {
            var data = EnumDropDownData.GetData(attribute.EnumType);
            Array enumValues = data.EnumValues;

            Enum lastElement = enumValues.GetValue(enumValues.Length - 1) as Enum;
            int length = Convert.ToInt32(lastElement) + 1;

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

            BitList bits = BitList.CreateFromBitMask(property.intValue, names.Length);
            EditorUtilityExt.DisplayMultiSelectableList(position, bits, names, onCloseMenu);

            void onCloseMenu(BitList bitList)
            {
                if (property.serializedObject.Disposed())
                    return;

                property.serializedObject.Update();
                property.intValue = bitList.ToIntBitMask();
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void ShowBitListMenu(in Rect position, SerializedProperty property)
        {
            var data = EnumDropDownData.GetData(attribute.EnumType);
            Array enumValues = data.EnumValues;

            Enum lastElement = enumValues.GetValue(enumValues.Length - 1) as Enum;
            string[] names = new string[Convert.ToInt32(lastElement) + 1];

            foreach (Enum item in enumValues)
            {
                names[Convert.ToInt32(item)] = item.GetName();
            }

            SerializedProperty arrayProp = property.FindPropertyRelative(BitList.ArrayFieldName);

            int intBlocksCount = BitList.GetArraySize(names.Length);
            arrayProp.arraySize = intBlocksCount;

            Span<int> intBlocks = stackalloc int[intBlocksCount];
            for (int i = 0; i < intBlocks.Length; i++)
            {
                intBlocks[i] = arrayProp.GetArrayElementAtIndex(i).intValue;
            }
            BitList bits = new BitList(intBlocks);
            bits.Count = names.Length;

            EditorUtilityExt.DisplayMultiSelectableList(position, bits, names, onCloseMenu);

            void onCloseMenu(BitList bitList)
            {
                if (property.serializedObject.Disposed())
                    return;

                property.serializedObject.Update();
                SerializedProperty arrayProp = property.FindPropertyRelative(BitList.ArrayFieldName);
                for (int i = 0; i < bitList.IntBlocks.Count; i++)
                {
                    arrayProp.GetArrayElementAtIndex(i).intValue = bitList.IntBlocks[i];
                }
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
