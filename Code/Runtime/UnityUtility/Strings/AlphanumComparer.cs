using System;

namespace UnityUtility.Strings
{
    /// <summary>
    /// Based on http://www.dotnetperls.com/alphanumeric-sorting
    /// </summary>
    [Serializable]
    public class AlphanumComparer : StringComparer
    {
        [NonSerialized]
        private char[] _bufCharArray;
        private StringComparison _comparison;

        public AlphanumComparer() : this(StringComparison.CurrentCulture)
        {

        }

        public AlphanumComparer(StringComparison comparison)
        {
            _comparison = comparison;
        }

        public override int Compare(string x, string y)
        {
            if (x == null || y == null)
                return 0;

            int marker1 = 0;
            int marker2 = 0;
            int maxLength = Math.Max(x.Length, y.Length);

            while (marker1 < x.Length && marker2 < y.Length)
            {
                if (_bufCharArray == null || _bufCharArray.Length < maxLength)
                    _bufCharArray = new char[maxLength];

                string string1 = GetSubstring(x, _bufCharArray, ref marker1);
                string string2 = GetSubstring(y, _bufCharArray, ref marker2);

                int result;

                if (int.TryParse(string1, out int num1) &&
                    int.TryParse(string2, out int num2))
                {
                    result = num1.CompareTo(num2);
                }
                else
                {
                    result = string.Compare(string1, string2, _comparison);
                }

                if (result != 0)
                    return result;
            }

            return x.Length.CompareTo(y.Length);
        }

        private static string GetSubstring(string compString, char[] bufArray, ref int marker)
        {
            char ch = compString[marker];
            bool isDigit = char.IsDigit(ch);
            int i = 0;

            do
            {
                bufArray[i++] = ch;

                if (++marker >= compString.Length)
                    break;

                ch = compString[marker];

            } while (char.IsDigit(ch) == isDigit);

            return new string(bufArray, 0, i);
        }
    }
}
