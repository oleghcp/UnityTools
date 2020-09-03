using System;
using UnityUtilityTools;

namespace UnityUtility.Collections.Unsafe
{
    public static class ArrayUtility
    {
        public static unsafe T[] ToArray<T>(T* array, int length) where T : unmanaged
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            T[] newArray = new T[length];

            for (int i = 0; i < length; i++)
            {
                newArray[i] = array[i];
            }

            return newArray;
        }

        public static unsafe void Sort<T>(T* array, int length) where T : unmanaged, IComparable<T>
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            ArrayHelper.QuickSort(array, 0, length - 1);
        }

        public static unsafe int Sum(int* array, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int sum = 0;

            for (int i = 0; i < length; i++)
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

            for (int i = 0; i < length; i++)
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
            if (array == null)
                throw new ArgumentNullException(nameof(array));

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
    }
}
