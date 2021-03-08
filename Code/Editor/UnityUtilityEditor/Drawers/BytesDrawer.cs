using UnityUtility;
using UnityUtility.MathExt;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(Bytes))]
    internal class BytesDrawer : PropertyDrawer
    {
        private bool _inited;
        private GUIContent[] _labels;
        private int[] _values;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_inited)
            {
                var lbl = new GUIContent();
                _labels = new GUIContent[Bytes.SIZE];
                for (int i = 0; i < Bytes.SIZE; i++)
                    _labels[i] = lbl;

                _values = new int[Bytes.SIZE];

                _inited = true;
            }

            SerializedProperty field = property.FindPropertyRelative(Bytes.SerFieldName);
            Bytes value = field.intValue;

            for (int i = 0; i < Bytes.SIZE; i++)
                _values[i] = value[i];

            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.MultiIntField(rect, _labels, _values);

            for (int i = 0; i < Bytes.SIZE; i++)
                value[i] = (byte)_values[i].Clamp(0, byte.MaxValue);

            field.intValue = (int)value;
        }
    }
}
