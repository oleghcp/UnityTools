using System;

namespace UU
{
    //This code is from https://www.pvladov.com/
    public static class ConvertUtility
    {
        private const string SYMBOLS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string DecimalToStringWithCustomRadix(long decimalNumber, int radix)
        {
            const int BITS_IN_LONG = 64;

            if (radix < 2 || radix > SYMBOLS.Length)
                throw new ArgumentException($"The radix must be >= 2 and <= {SYMBOLS.Length}");

            if (decimalNumber == 0)
                return "0";

            int index = BITS_IN_LONG - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BITS_IN_LONG];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = SYMBOLS[remainder];
                currentNumber /= radix;
            }

            string result = new string(charArray, index + 1, BITS_IN_LONG - index - 1);
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
            if (radix < 2 || radix > SYMBOLS.Length)
                throw new ArgumentException($"The radix must be >= 2 and <= {SYMBOLS.Length}");

            if (number.IsNullOrEmpty())
                return 0;

            // Make sure the arbitrary numeral system number is in upper case
            number = number.ToUpperInvariant();

            long result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                var c = number[i];
                if (i == 0 && c == '-')
                {
                    // This is the negative sign symbol
                    result = -result;
                    break;
                }

                int digit = SYMBOLS.IndexOf(c);
                if (digit == -1)
                    throw new ArgumentException("Invalid character in the arbitrary numeral system number", nameof(number));

                result += digit * multiplier;
                multiplier *= radix;
            }

            return result;
        }

        public static bool TryParseStringCustomRadixToDecimal(string number, int radix, out long result)
        {
            try
            {
                result = ParseStringCustomRadixToDecimal(number, radix);
                return true;
            }
            catch (ArgumentException)
            {
                result = default;
                return false;
            }
        }
    }
}
