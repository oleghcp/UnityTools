using UnityEditor;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtilityEditor
{
    internal static class EditorScriptUtility
    {
        public const string CATEGORY = nameof(UnityUtility);

        public static bool DrawCenterButton(string text, float w, float h)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            bool pressed = GUILayout.Button(text, GUILayout.MinWidth(w), GUILayout.MaxHeight(h));
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            return pressed;
        }

        public static void DrawCenterLabel(string text, float w)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel, GUILayout.Width(w));
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawWrongTypeMessage(Rect position, GUIContent label, string message)
        {
            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.LabelField(rect, message);
        }

        public static bool EqualizeSize(SerializedProperty arrayProp, int targetSize, object defVal)
        {
            bool changed = false;

            while (arrayProp.arraySize < targetSize)
            {
                arrayProp.InsertArrayElementAtIndex(arrayProp.arraySize);
                f_initProp(arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1), defVal);

                changed = true;
            }

            while (arrayProp.arraySize > targetSize)
            {
                arrayProp.DeleteArrayElementAtIndex(arrayProp.arraySize - 1);

                changed = true;
            }

            return changed;
        }

        private static void f_initProp(SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = (int)value;
                    break;

                case SerializedPropertyType.Float:
                    property.floatValue = (float)value;
                    break;

                case SerializedPropertyType.Enum:
                    property.enumValueIndex = (int)value;
                    break;
            }
        }

        public static void MoveToggles(BitArrayMask bits, int length, bool up)
        {
            if (up)
            {
                for (int i = 0; i < length; i++)
                {
                    f_toggleIteration(bits, -1, i, i > 0);
                }
            }
            else
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    f_toggleIteration(bits, 1, i, i < length - 1);
                }
            }
        }

        public static void MoveElements(SerializedProperty arrayProp, BitArrayMask bits, bool up, object defVal)
        {
            int length = arrayProp.arraySize;

            if (up)
            {
                for (int i = 0; i < length; i++)
                {
                    f_elementIteration(arrayProp, bits, defVal, -1, i, i > 0);
                }
            }
            else
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    f_elementIteration(arrayProp, bits, defVal, 1, i, i < length - 1);
                }
            }
        }

        private static void f_toggleIteration(BitArrayMask bits, int offset, int i, bool condition)
        {
            if (bits.Get(i))
            {
                bits.Set(i, false);

                if (condition)
                    bits.Set(i + offset, true);
            }
        }

        private static void f_elementIteration(SerializedProperty arrayProp, BitArrayMask bits, object defVal, int offset, int i, bool condition)
        {
            if (bits.Get(i))
            {
                if (condition)
                    arrayProp.MoveArrayElement(i, i + offset);
                else
                    f_initProp(arrayProp.GetArrayElementAtIndex(i), defVal);
            }
        }
    }
}
