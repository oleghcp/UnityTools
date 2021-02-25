using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityUtility;

namespace UnityEditor
{
    public static class UnityEditorExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ManagedReferenceValueIsNull(this SerializedProperty self)
        {
            return self.managedReferenceFullTypename.IsNullOrEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (string AssemblyName, string ClassName) GetManagedReferenceTypeName(this SerializedProperty self)
        {
            return EditorUtilityExt.SplitSerializedPropertyTypename(self.managedReferenceFullTypename);
        }

        public static void SortArray<T>(this SerializedProperty self, Func<SerializedProperty, T> selector)
        {
            Comparer<T> defComp = Comparer<T>.Default;

            int length = self.arraySize - 1;
            int len = length;

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    T a = selector(self.GetArrayElementAtIndex(j));
                    T b = selector(self.GetArrayElementAtIndex(j + 1));

                    if (defComp.Compare(a, b) > 0)
                    {
                        self.MoveArrayElement(j, j + 1);
                    }
                }
                len--;
            }
        }

        public static IEnumerable<SerializedProperty> EnumerateArrayElements(this SerializedProperty self)
        {
            int len = self.arraySize;

            for (int i = 0; i < len; i++)
            {
                yield return self.GetArrayElementAtIndex(i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddArrayElement(this SerializedProperty self)
        {
            self.InsertArrayElementAtIndex(self.arraySize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializedProperty PlaceArrayElement(this SerializedProperty self)
        {
            return self.PushArrayElementAtIndex(self.arraySize);
        }

        public static SerializedProperty PushArrayElementAtIndex(this SerializedProperty self, int index)
        {
            self.InsertArrayElementAtIndex(index);
            return self.GetArrayElementAtIndex(index);
        }

        public static void SetBytesValue(this SerializedProperty self, Bytes value)
        {
            using (var inner = self.FindPropertyRelative(Bytes.SerFieldName))
            {
                inner.intValue = (int)value;
            }
        }

        public static Bytes GetBytesValue(this SerializedProperty self)
        {
            Bytes value = default;

            using (var inner = self.FindPropertyRelative(Bytes.SerFieldName))
            {
                value = inner.intValue;
            }

            return value;
        }

        public static bool Disposed(this SerializedObject self)
        {
            try
            {
                bool check = self.isEditingMultipleObjects;
            }
            catch (Exception)
            {
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFolder(this DefaultAsset self)
        {
            return ProjectWindowUtil.IsFolder(self.GetInstanceID());
        }
    }
}
