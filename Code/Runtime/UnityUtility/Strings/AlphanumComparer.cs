using System;
using UnityUtility.CSharp;

namespace UnityUtility.Strings
{
    /// <summary>
    /// Based on http://www.dotnetperls.com/alphanumeric-sorting
    /// </summary>
    [Serializable]
    public class AlphanumComparer : StringComparer
    {
        [NonSerialized]
        private char[] _space1, _space2;
        private StringComparison _comparison;

        public AlphanumComparer(StringComparison comparison = StringComparison.CurrentCulture)
        {
            _comparison = comparison;
        }

        public override int Compare(string x, string y)
        {
            if (x == null || y == null)
                return 0;

            int marker1 = 0;
            int marker2 = 0;

            // Walk through two the strings with two markers.
            while (marker1 < x.Length && marker2 < y.Length)
            {
                UpdateArray(ref _space1, x.Length);
                UpdateArray(ref _space2, y.Length);

                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                int stringLength1 = FillSpaceArray(x, _space1, ref marker1);
                int stringLength2 = FillSpaceArray(y, _space2, ref marker2);

                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                string string1 = new string(_space1, 0, stringLength1);
                string string2 = new string(_space2, 0, stringLength2);

                int result;

                if (char.IsDigit(_space1[0]) && char.IsDigit(_space2[0]))
                {
                    int thisNumericChunk = int.Parse(string1);
                    int thatNumericChunk = int.Parse(string2);
                    result = thisNumericChunk.CompareTo(thatNumericChunk);
                }
                else
                {
                    result = string.Compare(string1.RemoveWhiteSpaces(),
                                            string2.RemoveWhiteSpaces(),
                                            _comparison);
                }

                if (result != 0)
                    return result;
            }

            return x.Length - y.Length;
        }

        private static void UpdateArray(ref char[] array, int length)
        {
            if (array == null || array.Length < length)
                array = new char[length];
        }

        private static int FillSpaceArray(string compString, char[] space, ref int marker)
        {
            int length = compString.Length;
            char ch = compString[marker];

            int i = 0;
            do
            {
                space[i++] = ch;
                marker++;

                if (marker < length)
                {
                    ch = compString[marker];
                }
                else
                {
                    break;
                }
            } while (char.IsDigit(ch) == char.IsDigit(space[0]));

            return i;
        }
    }
}
