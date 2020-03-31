using System;

namespace UnityUtility.Collections.Unsafe
{
    internal static class ArrayHelper
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
                {
                    T tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;

                    i++;
                    j--;
                }
            }

            if (left < j)
                QuickSort(array, left, j);

            if (i < right)
                QuickSort(array, i, right);
        }
    }
}
