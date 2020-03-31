using UnityUtility;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor
{
    internal class EnumerableSerializedProperty : IEnumerable<SerializedProperty>
    {
        internal SerializedProperty Prop;

        public IEnumerator<SerializedProperty> GetEnumerator()
        {
            int len = Prop.arraySize;

            for (int i = 0; i < len; i++)
            {
                yield return Prop.GetArrayElementAtIndex(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class UnityEditorExtensions
    {
        public static void SortArray<T>(this SerializedProperty prop, Func<SerializedProperty, T> selector)
        {
            Comparer<T> defComp = Comparer<T>.Default;

            int length = prop.arraySize - 1;
            int len = length;

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    T a = selector(prop.GetArrayElementAtIndex(j));
                    T b = selector(prop.GetArrayElementAtIndex(j + 1));

                    if (defComp.Compare(a, b) > 0)
                    {
                        prop.MoveArrayElement(j, j + 1);
                    }
                }
                len--;
            }
        }

        public static IEnumerable<SerializedProperty> GetArray(this SerializedProperty prop)
        {
            return new EnumerableSerializedProperty { Prop = prop };
        }

        public static void SetBytesValue(this SerializedProperty prop, Bytes value)
        {
            using (var inner = prop.FindPropertyRelative(Bytes.SerFieldName))
            {
                inner.intValue = (int)value;
            }
        }

        public static Bytes GetBytesValue(this SerializedProperty prop)
        {
            Bytes value = default;

            using (var inner = prop.FindPropertyRelative(Bytes.SerFieldName))
            {
                value = inner.intValue;
            }

            return value;
        }

        public static bool Disposed(this SerializedObject serializedObject)
        {
            try
            {
                bool check = serializedObject.isEditingMultipleObjects;
            }
            catch (Exception)
            {
                return true;
            }

            return false;
        }
    }
}
