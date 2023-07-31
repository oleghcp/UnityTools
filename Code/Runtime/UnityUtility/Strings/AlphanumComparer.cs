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
        private char[] _bufCharArray;
        private StringComparison _comparison;

        public AlphanumComparer()
        {
            _comparison = StringComparison.CurrentCulture;
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
                UpdateBufArray(maxLength);

                string string1 = GetSubstring(x, ref marker1);
                string string2 = GetSubstring(y, ref marker2);

                int result;

                if (int.TryParse(string1, out int num1) &&
                    int.TryParse(string2, out int num2))
                {
                    result = num1.CompareTo(num2);
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

            return x.Length.CompareTo(y.Length);
        }

        private void UpdateBufArray(int length)
        {
            if (_bufCharArray == null || _bufCharArray.Length < length)
                _bufCharArray = new char[length];
        }

        private string GetSubstring(string compString, ref int marker)
        {
            char ch = compString[marker];
            bool isDigit = char.IsDigit(ch);
            int i = 0;

            do
            {
                _bufCharArray[i++] = ch;

                if (++marker >= compString.Length)
                    break;

                ch = compString[marker];

            } while (char.IsDigit(ch) == isDigit);

            return new string(_bufCharArray, 0, i);
        }
    }
}
