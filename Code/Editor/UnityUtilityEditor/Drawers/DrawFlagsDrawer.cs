using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtilityEditor.Window.BitArrays;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawFlagsAttribute))]
    internal class DrawFlagsDrawer : AttributeDrawer<DrawFlagsAttribute>
    {
        private Data _data;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_data == null)
                _data = new Data(property, this);

            _data.Draw(position, property, label);
        }

        private class Data
        {
            private readonly string _error;

            private readonly GUIContent _label;
            private readonly string[] _names;
            private readonly bool _isBitArray;

            private EnumBitArrayMaskWindow _window;

            public Data(SerializedProperty property, AttributeDrawer<DrawFlagsAttribute> drawer)
            {
                Type type = EditorUtilityExt.GetFieldType(drawer);

                if (type != typeof(BitList) && type.GetTypeCode() != TypeCode.Int32)
                {
                    _error = $"Use {nameof(DrawFlagsAttribute)} with Int32 or BitArrayMask.";
                    return;
                }

                _label = new GUIContent(property.displayName);
                Array values = Enum.GetValues(drawer.attribute.EnumType);
                _isBitArray = type == typeof(BitList);

                if (!_isBitArray && values.Length > BitMask.SIZE)
                {
                    _error = $"Enum values amount cannot be more than {BitMask.SIZE}";
                    return;
                }

                Enum lastElement = values.GetValue(values.Length - 1) as Enum;
                _names = new string[Convert.ToInt32(lastElement) + 1];

                foreach (Enum item in values)
                {
                    _names[Convert.ToInt32(item)] = item.GetName();
                }

                if (_isBitArray)
                {
                    EnumBitArrayMaskWindow.CheckArray(property, _names);
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            public void Draw(Rect position, SerializedProperty property, GUIContent label)
            {
                if (_error != null)
                {
                    EditorGui.ErrorLabel(position, label, _error);
                    return;
                }

                if (_isBitArray)
                    BitListDrawer.Draw(position, _label, ref _window, Tuple.Create(property, _names));
                else
                    property.intValue = EditorGUI.MaskField(position, _label, property.intValue, _names);
            }
        }
    }
}
