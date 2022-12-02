#if !UNITY_2021_2_OR_NEWER
using System;
using System.Collections.Generic;
using UnityUtility.Tools;

namespace UnityUtility
{
    internal static class UnsafeArrayUtility
    {
        public static unsafe void QuickSort<T>(T* array, int left, int right) where T : unmanaged, IComparable<T>
        {
            int i = left, j = right;
            T pivot = array[(left + right) / 2];

            while (i < j)
            {
                while (array[i].CompareTo(pivot) < 0) { i++; }
                while (array[j].CompareTo(pivot) > 0) { j--; }

                if (i <= j)
                    Helper.Swap(ref array[i++], ref array[j--]);
            }

            if (left < j)
                QuickSort(array, left, j);

            if (i < right)
                QuickSort(array, i, right);
        }

        public static unsafe void QuickSort<T>(T* array, int left, int right, Comparison<T> comparer) where T : unmanaged
        {
            int i = left, j = right;
            T pivot = array[(left + right) / 2];

            while (i < j)
            {
                while (comparer(array[i], pivot) < 0) { i++; }
                while (comparer(array[j], pivot) > 0) { j--; }

                if (i <= j)
                    Helper.Swap(ref array[i++], ref array[j--]);
            }

            if (left < j)
                QuickSort(array, left, j, comparer);

            if (i < right)
                QuickSort(array, i, right, comparer);
        }

        public static unsafe void QuickSort<T, TComparer>(T* array, int left, int right, TComparer comparer)
            where T : unmanaged
            where TComparer : IComparer<T>
        {
            int i = left, j = right;
            T pivot = array[(left + right) / 2];

            while (i < j)
            {
                while (comparer.Compare(array[i], pivot) < 0) { i++; }
                while (comparer.Compare(array[j], pivot) > 0) { j--; }

                if (i <= j)
                    Helper.Swap(ref array[i++], ref array[j--]);
            }

            if (left < j)
                QuickSort(array, left, j, comparer);

            if (i < right)
                QuickSort(array, i, right, comparer);
        }

        public static unsafe void QuickSort<T, TKey>(T* array, int left, int right, Func<T, TKey> selector)
            where T : unmanaged
            where TKey : IComparable<TKey>
        {
            int i = left, j = right;
            TKey pivotKey = selector(array[(left + right) / 2]);

            while (i < j)
            {
                while (selector(array[i]).CompareTo(pivotKey) < 0) { i++; }
                while (selector(array[j]).CompareTo(pivotKey) > 0) { j--; }

                if (i <= j)
                    Helper.Swap(ref array[i++], ref array[j--]);
            }

            if (left < j)
                QuickSort(array, left, j, selector);

            if (i < right)
                QuickSort(array, i, right, selector);
        }
    }
}
#endif
