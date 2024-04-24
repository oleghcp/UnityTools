using System.Collections.Generic;

namespace OlegHcp.CSharp
{
    internal static class ArrayUtility
    {
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
    }
}
