using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawBitMaskAttribute))]
    internal class DrawBitMaskDrawer : PropertyDrawer
    {
        private class Data
        {
            private readonly string m_error;

            private readonly GUIContent m_label;
            private readonly string[] m_names;
            private readonly bool m_array;

            private EnumBitArrayMaskWindow m_win;

            public Data(SerializedProperty property, PropertyDrawer drawer)
            {
                Array values;

                if (drawer.fieldInfo.FieldType == typeof(BitArrayMask))
                {
                    m_label = new GUIContent(property.displayName);
                    values = Enum.GetValues((drawer.attribute as DrawBitMaskAttribute).EnumType);
                    m_array = true;
                }
                else if (drawer.fieldInfo.FieldType.GetTypeCode() == TypeCode.Int32)
                {
                    values = Enum.GetValues((drawer.attribute as DrawBitMaskAttribute).EnumType);

                    if (values.Length > BitMask.SIZE)
                    {
                        m_error = "Enum values amount cannot be more than " + BitMask.SIZE.ToString();
                        return;
                    }

                    m_label = new GUIContent(property.displayName);
                }
                else
                {
                    m_error = "Use DrawBitMask with Int32 or BitArrayMask.";
                    return;
                }

                Enum lastElement = values.GetValue(values.Length - 1) as Enum;
                m_names = new string[lastElement.ToInteger() + 1];

                foreach (Enum item in values)
                {
                    m_names[item.ToInteger()] = item.GetName();
                }

                if (m_array)
                {
                    EnumBitArrayMaskWindow.CheckArray(property, m_names);
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            public void Draw(Rect position, SerializedProperty property, GUIContent label)
            {
                if (m_error != null)
                {
                    EditorScriptUtility.DrawWrongTypeMessage(position, label, m_error);
                    return;
                }

                if (m_array)
                    BitArrayMaskDrawer.Draw(position, m_label, ref m_win, Tuple.Create(property, m_names));
                else
                    property.intValue = EditorGUI.MaskField(position, m_label, property.intValue, m_names);
            }
        }

        private Data m_data;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_data == null)
                m_data = new Data(property, this);

            m_data.Draw(position, property, label);
        }
    }
}
