using System;
using UnityUtilityTools;

namespace UnityUtility
{
    internal static class UnsafeArrayUtility
    {
        public static unsafe void Reverse<T>(T* array, int startIndex, int length) where T : unmanaged
        {
            int backIndex = startIndex + length - 1;
            length /= 2;

            for (int i = 0; i < length; i++)
            {
                T tmp = array[startIndex + i];
                array[startIndex + i] = array[backIndex - i];
                array[backIndex - i] = tmp;
            }
        }

        public static unsafe void Sort<T>(T* array, int length) where T : unmanaged, IComparable<T>
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            f_quickSort(array, 0, length - 1);
        }

        public static unsafe void Sort<T>(T* array, int length, Comparison<T> comparer) where T : unmanaged
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            f_quickSort(array, 0, length - 1, comparer);
        }

        public static unsafe void Shuffle<T>(T* array, int length, IRng generator) where T : unmanaged
        {
            var last = length;

            while (last > 1)
            {
                var cur = generator.Next(last--);

                var value = array[cur];
                array[cur] = array[last];
                array[last] = value;
            }
        }

        public static unsafe void Shuffle<T>(T* array, int length) where T : unmanaged
        {
            var last = length;

            while (last > 1)
            {
                var cur = UnityEngine.Random.Range(0, last--);

                var value = array[cur];
                array[cur] = array[last];
                array[last] = value;
            }
        }

        public static unsafe int Sum(int* array, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            var sum = 0;

            for (var i = 0; i < length; i++)
            {
                sum += array[i];
            }

            return sum;
        }

        public static unsafe float Sum(float* array, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            float sum = 0;

            for (var i = 0; i < length; i++)
            {
                sum += array[i];
            }

            return sum;
        }

        public static unsafe T Min<T>(T* array, int length) where T : unmanaged, IComparable<T>
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (length <= 0)
                throw Errors.NoElements();

            var num = *array;

            for (var i = 1; i < length; i++)
            {
                if (array[i].CompareTo(num) < 0)
                    num = array[i];
            }

            return num;
        }

        public static unsafe T Max<T>(T* array, int length) where T : unmanaged, IComparable<T>
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (length <= 0)
                throw Errors.NoElements();

            var num = *array;

            for (var i = 1; i < length; i++)
            {
                if (array[i].CompareTo(num) > 0)
                    num = array[i];
            }

            return num;
        }

        private static unsafe void f_quickSort<T>(T* array, int left, int right) where T : unmanaged, IComparable<T>
        {
            int i = left, j = right;
            var pivot = array[(left + right) / 2];

            while (i < j)
            {
                while (array[i].CompareTo(pivot) < 0) { i++; }
                while (array[j].CompareTo(pivot) > 0) { j--; }

                if (i <= j)
                {
                    var tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;

                    i++;
                    j--;
                }
            }

            if (left < j)
                f_quickSort(array, left, j);

            if (i < right)
                f_quickSort(array, i, right);
        }

        private static unsafe void f_quickSort<T>(T* array, int left, int right, Comparison<T> comparer) where T : unmanaged
        {
            int i = left, j = right;
            var pivot = array[(left + right) / 2];

            while (i < j)
            {
                while (comparer(array[i], pivot) < 0) { i++; }
                while (comparer(array[j], pivot) > 0) { j--; }

                if (i <= j)
                {
                    var tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;

                    i++;
                    j--;
                }
            }

            if (left < j)
                f_quickSort(array, left, j, comparer);

            if (i < right)
                f_quickSort(array, i, right, comparer);
        }
    }
}
