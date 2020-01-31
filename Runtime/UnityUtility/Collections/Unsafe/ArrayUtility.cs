using System;

namespace UU.Collections.Unsafe
{
    public static class ArrayUtility
    {
        public static unsafe void Sort<T>(T* array, int length) where T : unmanaged, IComparable<T>
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Pointer cannot be null.");

            ArrayHelper.QuickSort(array, 0, length - 1);
        }

        public static unsafe int Sum(int* array, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Pointer cannot be null.");

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
                throw new ArgumentNullException(nameof(array), "Pointer cannot be null.");

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
                throw new ArgumentNullException(nameof(array), "Pointer cannot be null.");

            if (length <= 0)
                throw new InvalidOperationException("Array is empty.");

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
                throw new ArgumentNullException(nameof(array), "Pointer cannot be null.");

            if (length <= 0)
                throw new InvalidOperationException("Array is empty.");

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
