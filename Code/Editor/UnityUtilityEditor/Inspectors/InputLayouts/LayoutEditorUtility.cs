using UnityEditor;
using UnityUtility.Collections;

namespace UnityUtilityEditor.Inspectors.InputLayouts
{
    internal class LayoutEditorUtility
    {
        public static bool EqualizeSize(SerializedProperty arrayProp, int targetSize, object defVal)
        {
            bool changed = false;

            while (arrayProp.arraySize < targetSize)
            {
                InitProperty(arrayProp.PlaceArrayElement(), defVal);

                changed = true;
            }

            while (arrayProp.arraySize > targetSize)
            {
                arrayProp.DeleteArrayElementAtIndex(arrayProp.arraySize - 1);

                changed = true;
            }

            return changed;
        }

        public static void MoveToggles(BitList bits, int length, bool up)
        {
            if (up)
            {
                for (int i = 0; i < length; i++)
                {
                    ToggleIteration(bits, -1, i, i > 0);
                }
            }
            else
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    ToggleIteration(bits, 1, i, i < length - 1);
                }
            }
        }

        public static void MoveElements(SerializedProperty arrayProp, BitList bits, bool up, object defVal)
        {
            int length = arrayProp.arraySize;

            if (up)
            {
                for (int i = 0; i < length; i++)
                {
                    ElementIteration(arrayProp, bits, defVal, -1, i, i > 0);
                }
            }
            else
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    ElementIteration(arrayProp, bits, defVal, 1, i, i < length - 1);
                }
            }
        }

        private static void ToggleIteration(BitList bits, int offset, int i, bool condition)
        {
            if (bits.Get(i))
            {
                bits.Set(i, false);

                if (condition)
                    bits.Set(i + offset, true);
            }
        }

        private static void ElementIteration(SerializedProperty arrayProp, BitList bits, object defVal, int offset, int i, bool condition)
        {
            if (bits.Get(i))
            {
                if (condition)
                    arrayProp.MoveArrayElement(i, i + offset);
                else
                    InitProperty(arrayProp.GetArrayElementAtIndex(i), defVal);
            }
        }

        private static void InitProperty(SerializedProperty property, object value)
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
    }
}
