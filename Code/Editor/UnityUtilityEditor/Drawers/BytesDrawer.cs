using UnityUtility;
using UnityUtility.MathExt;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(Bytes))]
    internal class BytesDrawer : PropertyDrawer
    {
        private bool m_inited;
        private GUIContent[] m_labels;
        private int[] m_values;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_inited)
            {
                var lbl = new GUIContent();
                m_labels = new GUIContent[Bytes.SIZE];
                for (int i = 0; i < Bytes.SIZE; i++)
                    m_labels[i] = lbl;

                m_values = new int[Bytes.SIZE];

                m_inited = true;
            }

            SerializedProperty field = property.FindPropertyRelative(Bytes.SerFieldName);
            Bytes value = field.intValue;

            for (int i = 0; i < Bytes.SIZE; i++)
                m_values[i] = value[i];

            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.MultiIntField(rect, m_labels, m_values);

            for (int i = 0; i < Bytes.SIZE; i++)
                value[i] = (byte)m_values[i].Clamp(0, byte.MaxValue);

            field.intValue = (int)value;
        }
    }
}
