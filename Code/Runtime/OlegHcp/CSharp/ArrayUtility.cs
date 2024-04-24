using System;
using System.Collections.Generic;

namespace OlegHcp.CSharp
{
    internal static class ArrayUtility
    {
        #region Sort
        public static void Sort<T>(T[] array, int left, int right) where T : IComparable<T>
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = array[left];

            while (i <= j)
            {
                while (array[i].CompareTo(pivot) < 0) { i++; }
                while (array[j].CompareTo(pivot) > 0) { j--; }

                if (i <= j)
                    array.Swap(i++, j--);
            }

            Sort(array, left, j);
            Sort(array, i, right);
        }

        public static void Sort<T, TComp>(T[] array, int left, int right, TComp comparer) where TComp : IComparer<T>
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = array[left];

            while (i <= j)
            {
                while (comparer.Compare(array[i], pivot) < 0) { i++; }
                while (comparer.Compare(array[j], pivot) > 0) { j--; }

                if (i <= j)
                    array.Swap(i++, j--);
            }

            Sort(array, left, j, comparer);
            Sort(array, i, right, comparer);
        }

        public static void Sort<T>(T[] array, int left, int right, Comparison<T> comparison)
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = array[left];

            while (i <= j)
            {
                while (comparison(array[i], pivot) < 0) { i++; }
                while (comparison(array[j], pivot) > 0) { j--; }

                if (i <= j)
                    array.Swap(i++, j--);
            }

            Sort(array, left, j, comparison);
            Sort(array, i, right, comparison);
        }
        #endregion
    }
}
