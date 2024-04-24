using System;
using System.Collections.Generic;

namespace OlegHcp.CSharp
{
    internal static class ArrayUtility
    {
        #region Sort
        public static void Sort<T>(T[] collection, int left, int right) where T : IComparable<T>
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = collection[left];

            while (i <= j)
            {
                while (collection[i].CompareTo(pivot) < 0) { i++; }
                while (collection[j].CompareTo(pivot) > 0) { j--; }

                if (i <= j)
                    collection.Swap(i++, j--);
            }

            Sort(collection, left, j);
            Sort(collection, i, right);
        }

        public static void Sort<T, TComp>(T[] collection, int left, int right, TComp comparer) where TComp : IComparer<T>
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = collection[left];

            while (i <= j)
            {
                while (comparer.Compare(collection[i], pivot) < 0) { i++; }
                while (comparer.Compare(collection[j], pivot) > 0) { j--; }

                if (i <= j)
                    collection.Swap(i++, j--);
            }

            Sort(collection, left, j, comparer);
            Sort(collection, i, right, comparer);
        }

        public static void Sort<T>(T[] collection, int left, int right, Comparison<T> comparison)
        {
            if (left >= right)
                return;

            int i = left, j = right;
            T pivot = collection[left];

            while (i <= j)
            {
                while (comparison(collection[i], pivot) < 0) { i++; }
                while (comparison(collection[j], pivot) > 0) { j--; }

                if (i <= j)
                    collection.Swap(i++, j--);
            }

            Sort(collection, left, j, comparison);
            Sort(collection, i, right, comparison);
        }
        #endregion
    }
}
