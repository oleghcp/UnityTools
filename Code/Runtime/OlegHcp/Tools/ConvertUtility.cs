using System;
using OlegHcp.CSharp;

namespace OlegHcp.Tools
{
    //This code is from https://www.pvladov.com/
    public static class ConvertUtility
    {
        private static readonly string _symbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string DecimalToStringWithCustomRadix(long decimalNumber, int radix)
        {
            const int bitsInLong = 64;

            if (radix < 2 || radix > _symbols.Length)
                throw ThrowErrors.RadixOutOfRange(nameof(radix), _symbols.Length);

            if (decimalNumber == 0)
                return "0";

            int index = bitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[bitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = _symbols[remainder];
                currentNumber /= radix;
            }

            string result = new string(charArray, index + 1, bitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        /// <summary>
        /// Converts the given number from the numeral system with the specified
        /// radix (in the range [2, 36]) to decimal numeral system.
        /// </summary>
        /// <param name="number">The arbitrary numeral system number to convert.</param>
        /// <param name="radix">The radix of the numeral system the given number
        /// is in (in the range [2, 36]).</param>
        /// <returns></returns>
        public static long ParseStringCustomRadixToDecimal(string number, int radix)
        {
            if (number == null)
                throw ThrowErrors.NullParameter(nameof(number));

            if (radix < 2 || radix > _symbols.Length)
                throw ThrowErrors.RadixOutOfRange(nameof(radix), _symbols.Length);

            if (number == string.Empty)
                throw ThrowErrors.IncorrectInputString();

            if (TryParse(number, radix, out long result))
                return result;

            throw ThrowErrors.IncorrectInputString();
        }

        public static bool TryParseStringCustomRadixToDecimal(string number, int radix, out long result)
        {
            if (radix < 2 || radix > _symbols.Length || number.IsNullOrEmpty())
            {
                result = 0;
                return false;
            }

            return TryParse(number, radix, out result);
        }

        private static bool TryParse(string number, int radix, out long result)
        {
            // Make sure the arbitrary numeral system number is in upper case
            number = number.ToUpperInvariant();

            result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char c = number[i];
                if (i == 0 && c == '-')
                {
                    // This is the negative sign symbol
                    result = -result;
                    break;
                }

                int digit = _symbols.IndexOf(c);

                if (digit == -1)
                {
                    result = 0;
                    return false;
                }

                result += digit * multiplier;
                multiplier *= radix;
            }

            return true;
        }
    }
}
