using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtilityEditor.Window.BitArrays;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawBitMaskAttribute))]
    internal class DrawBitMaskDrawer : AttributeDrawer<DrawBitMaskAttribute>
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

            public Data(SerializedProperty property, AttributeDrawer<DrawBitMaskAttribute> drawer)
            {
                Type type = EditorUtilityExt.GetFieldType(drawer.fieldInfo);

                if (type != typeof(BitArrayMask) && type.GetTypeCode() != TypeCode.Int32)
                {
                    _error = $"Use {nameof(DrawBitMaskAttribute)} with Int32 or BitArrayMask.";
                    return;
                }

                _label = new GUIContent(property.displayName);
                Array values = Enum.GetValues(drawer.attribute.EnumType);
                _isBitArray = type == typeof(BitArrayMask);

                if (!_isBitArray && values.Length > BitMask.SIZE)
                {
                    _error = $"Enum values amount cannot be more than {BitMask.SIZE}";
                    return;
                }

                Enum lastElement = values.GetValue(values.Length - 1) as Enum;
                _names = new string[lastElement.ToInteger() + 1];

                foreach (Enum item in values)
                {
                    _names[item.ToInteger()] = item.GetName();
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
                    GUIExt.DrawWrongTypeMessage(position, label, _error);
                    return;
                }

                if (_isBitArray)
                    BitArrayDrawer.Draw(position, _label, ref _window, Tuple.Create(property, _names));
                else
                    property.intValue = EditorGUI.MaskField(position, _label, property.intValue, _names);
            }
        }
    }
}
