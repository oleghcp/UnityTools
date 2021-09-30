#if UNITY_2018_3_OR_NEWER
using System;
using UnityUtilityTools;

namespace UnityUtility
{
    internal static class UnsafeArrayUtility
    {
        public static unsafe void Shuffle<T>(T* array, int length, IRng generator) where T : unmanaged
        {
            int last = length;

            while (last > 1)
            {
                int cur = generator.Next(last--);
                Helper.Swap(ref array[cur], ref array[last]);
            }
        }

        public static unsafe void Shuffle<T>(T* array, int length) where T : unmanaged
        {
            int last = length;

            while (last > 1)
            {
                int cur = UnityEngine.Random.Range(0, last--);
                Helper.Swap(ref array[cur], ref array[last]);
            }
        }

        public static unsafe T Min<T>(T* array, int length) where T : unmanaged, IComparable<T>
        {
            if (length <= 0)
                throw Errors.NoElements();

            T num = *array;

            for (int i = 1; i < length; i++)
            {
                if (array[i].CompareTo(num) < 0)
                    num = array[i];
            }

            return num;
        }

        public static unsafe T Max<T>(T* array, int length) where T : unmanaged, IComparable<T>
        {
            if (length <= 0)
                throw Errors.NoElements();

            T num = *array;

            for (int i = 1; i < length; i++)
            {
                if (array[i].CompareTo(num) > 0)
                    num = array[i];
            }

            return num;
        }

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
    }
}
#endif
