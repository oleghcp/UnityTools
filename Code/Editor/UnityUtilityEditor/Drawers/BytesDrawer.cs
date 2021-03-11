using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(Bytes))]
    internal class BytesDrawer : PropertyDrawer
    {
        private GUIContent[] _labels;
        private int[] _values;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_values == null)
            {
                _values = new int[Bytes.SIZE];

                _labels = new GUIContent[Bytes.SIZE];
                for (int i = 0; i < Bytes.SIZE; i++)
                {
                    _labels[i] = new GUIContent();
                }
            }

            SerializedProperty field = property.FindPropertyRelative(Bytes.FieldName);
            Bytes value = field.intValue;

            for (int i = 0; i < Bytes.SIZE; i++)
            {
                _values[i] = value[i];
            }

            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.MultiIntField(rect, _labels, _values);

            for (int i = 0; i < Bytes.SIZE; i++)
            {
                value[i] = (byte)_values[i].Clamp(0, byte.MaxValue);
            }

            field.intValue = (int)value;
        }
    }
}
