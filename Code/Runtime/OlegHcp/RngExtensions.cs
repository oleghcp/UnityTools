using System;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.Collections;
using OlegHcp.CSharp;
using OlegHcp.Mathematics;
using OlegHcp.NumericEntities;
using OlegHcp.Strings;
using OlegHcp.Tools;

namespace OlegHcp
{
    public static class RngExtensions
    {
        public static int Next(this IRng self, in DiapasonInt range)
        {
            return self.Next(range.Min, range.Max);
        }

        public static float Next(this IRng self, in Diapason range)
        {
            return self.Next(range.Min, range.Max);
        }

        public static int Next(this IRng self, in (int min, int max) range)
        {
            return self.Next(range.min, range.max);
        }

        public static float Next(this IRng self, in (float min, float max) range)
        {
            return self.Next(range.min, range.max);
        }

        /// <summary>
        /// Returns true with chance from 0f to 1f.
        /// </summary>
        public static bool Chance(this IRng self, float likelihood)
        {
            if (likelihood == 0f)
                return false;

            return likelihood >= 1f || likelihood > self.Next(0f, 1f);
        }

        /// <summary>
        /// Returns random index of array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, float[] weights, int weightOfNone = 0)
        {
#if UNITY_2021_2_OR_NEWER || !UNITY
            return Random(self, (Span<float>)weights, weightOfNone);
#else
            return Random(self, (IList<float>)weights, weightOfNone);
#endif
        }

        /// <summary>
        /// Returns random index of collection contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, IList<float> weights, float weightOfNone = 0f)
        {
            if (weights.Count > 0)
            {
                float sum = weights.Sum();

                float target;
                do { target = self.Next(sum + weightOfNone); }
                while (target == sum + weightOfNone);

                int startIndex = self.Next(weights.Count);
                int count = weights.Count + startIndex;
                sum = 0f;

                for (int i = startIndex; i < count; i++)
                {
                    int index = i % weights.Count;

                    if (weights[index] + sum > target)
                        return index;

                    sum += weights[index];
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, Span<float> weights, float weightOfNone = 0f)
        {
            if (weights.Length > 0)
            {
                float sum = weights.Sum();

                float target;
                do { target = self.Next(sum + weightOfNone); }
                while (target == sum + weightOfNone);

                int startIndex = self.Next(weights.Length);
                int count = weights.Length + startIndex;
                sum = 0f;

                for (int i = startIndex; i < count; i++)
                {
                    int index = i % weights.Length;

                    if (weights[index] + sum > target)
                        return index;

                    sum += weights[index];
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, int[] weights, int weightOfNone = 0)
        {
#if UNITY_2021_2_OR_NEWER || !UNITY
            return Random(self, (Span<int>)weights, weightOfNone);
#else
            return Random(self, (IList<int>)weights, weightOfNone);
#endif
        }

        /// <summary>
        /// Returns random index of collection contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, IList<int> weights, int weightOfNone = 0)
        {
            if (weights.Count > 0)
            {
                int target = self.Next(weights.Sum() + weightOfNone);
                int startIndex = self.Next(weights.Count);
                int count = weights.Count + startIndex;
                int sum = 0;

                for (int i = startIndex; i < count; i++)
                {
                    int index = i % weights.Count;

                    if (weights[index] + sum > target)
                        return index;

                    sum += weights[index];
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, Span<int> weights, int weightOfNone = 0)
        {
            if (weights.Length > 0)
            {
                int target = self.Next(weights.Sum() + weightOfNone);
                int startIndex = self.Next(weights.Length);
                int count = weights.Length + startIndex;
                int sum = 0;

                for (int i = startIndex; i < count; i++)
                {
                    int index = i % weights.Length;

                    if (weights[index] + sum > target)
                        return index;

                    sum += weights[index];
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a random flag index contained in the specified mask. Returns -1 if mask is empty.
        /// </summary>
        /// <param name="length">How many flags of 32bit mask should be considered.</param>
        public static int RandomFlag(this IRng self, int mask, int length)
        {
            int rn = self.Next(BitMask.GetCount(mask, length));

            for (int i = 0; i < length; i++)
            {
                if (BitMask.HasFlag(mask, i))
                {
                    if (rn-- == 0)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a random flag index contained in the specified mask. Returns -1 if mask is empty.
        /// </summary>
        public static int RandomFlag(this IRng self, BitList mask)
        {
            int rn = self.Next(mask.GetFlagsCount());

            for (int i = 0; i < mask.Length; i++)
            {
                if (mask.Get(i))
                {
                    if (rn-- == 0)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to max values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float Ascending(this IRng self, float min, float max, float offsetIntensity = 1f)
        {
            if (min < max)
                return AscendingInternal(self, min, max, offsetIntensity);

            if (min == max)
                return min;

            throw ThrowErrors.MinMax(nameof(min), nameof(max));
        }

        private static float AscendingInternal(IRng self, float min, float max, float offsetIntensity = 1f)
        {
            float range = max - min;
            float rnd = self.Next(0f, 1f);
            return (1f - rnd.Pow(offsetIntensity.ClampMin(0f) + 1f)) * range + min;
        }

        /// <summary>
        /// Returns a random float number within range (min/max inclusive) with chance offset to max values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float Ascending(this IRng self, in Diapason range, float offsetIntensity = 1f)
        {
            return Ascending(self, range.Min, range.Max, offsetIntensity);
        }

        /// <summary>
        /// Returns a random float number within range (min/max inclusive) with chance offset to max values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float Ascending(this IRng self, in (float min, float max) range, float offsetIntensity = 1f)
        {
            return Ascending(self, range.min, range.max, offsetIntensity);
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float Descending(this IRng self, float min, float max, float offsetIntensity = 1f)
        {
            if (min < max)
                return DescendingInternal(self, min, max, offsetIntensity);

            if (min == max)
                return min;

            throw ThrowErrors.MinMax(nameof(min), nameof(max));
        }

        private static float DescendingInternal(IRng self, float min, float max, float offsetIntensity = 1f)
        {
            float range = max - min;
            float rnd = self.Next(0f, 1f);
            return rnd.Pow(offsetIntensity.ClampMin(0f) + 1f) * range + min;
        }

        /// <summary>
        /// Returns a random float number within range (min/max inclusive) with chance offset to min values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float Descending(this IRng self, in Diapason range, float offsetIntensity = 1f)
        {
            return Descending(self, range.Min, range.Max, offsetIntensity);
        }

        /// <summary>
        /// Returns a random float number within range (min/max inclusive) with chance offset to min values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float Descending(this IRng self, in (float min, float max) range, float offsetIntensity = 1f)
        {
            return Descending(self, range.min, range.max, offsetIntensity);
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min and max values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float MinMax(this IRng self, float min, float max, float offsetIntensity = 1f)
        {
            if (min < max)
            {
                return self.Next(0, 2) == 0 ? AscendingInternal(self, (max + min) * 0.5f, max, offsetIntensity)
                                            : DescendingInternal(self, min, (max + min) * 0.5f, offsetIntensity);
            }

            if (min == max)
                return min;

            throw ThrowErrors.MinMax(nameof(min), nameof(max));
        }

        /// <summary>
        /// Returns a random float number within range (min/max inclusive) with chance offset to min and max values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float MinMax(this IRng self, in Diapason range, float offsetIntensity = 1f)
        {
            return MinMax(self, range.Min, range.Max, offsetIntensity);
        }

        /// <summary>
        /// Returns a random float number within range (min/max inclusive) with chance offset to min and max values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float MinMax(this IRng self, in (float min, float max) range, float offsetIntensity = 1f)
        {
            return MinMax(self, range.min, range.max, offsetIntensity);
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to average values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float Average(this IRng self, float min, float max, float offsetIntensity = 1f)
        {
            if (min < max)
            {
                return self.Next(0, 2) == 0 ? AscendingInternal(self, min, (max + min) * 0.5f, offsetIntensity)
                                            : DescendingInternal(self, (max + min) * 0.5f, max, offsetIntensity);
            }

            if (min == max)
                return min;

            throw ThrowErrors.MinMax(nameof(min), nameof(max));
        }

        /// <summary>
        /// Returns a random float number within range (min/max inclusive) with chance offset to average values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float Average(this IRng self, in Diapason range, float offsetIntensity = 1f)
        {
            return Average(self, range.Min, range.Max, offsetIntensity);
        }

        /// <summary>
        /// Returns a random float number within range (min/max inclusive) with chance offset to average values.
        /// </summary>
        /// <param name="offsetIntensity">Offset intensity from zero to infinity. There is no offset if intensity is zero.</param>
        public static float Average(this IRng self, in (float min, float max) range, float offsetIntensity = 1f)
        {
            return Average(self, range.min, range.max, offsetIntensity);
        }

        /// <summary>
        /// Returns a random even integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int RandomEven(this IRng self, int min, int max)
        {
            if (min % 2 != 0)
            {
                if (max - min < 2)
                    throw ThrowErrors.RangeDoesNotContain("even");

                min++;
            }

            return self.Next(min, max) & -2;
        }

        /// <summary>
        /// Returns a random even integer number within range (min [inclusive] and max [exclusive]).
        /// </summary>
        public static int RandomEven(this IRng self, in DiapasonInt range)
        {
            return RandomEven(self, range.Min, range.Max);
        }

        /// <summary>
        /// Returns a random even integer number within range (min [inclusive] and max [exclusive]).
        /// </summary>
        public static int RandomEven(this IRng self, in (int min, int max) range)
        {
            return RandomEven(self, range.min, range.max);
        }

        /// <summary>
        /// Returns a random odd integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int RandomOdd(this IRng self, int min, int max)
        {
            if (min % 2 == 0)
            {
                if (min == max)
                    throw ThrowErrors.RangeDoesNotContain("odd");
            }
            else if (max - min < 2)
            {
                throw ThrowErrors.RangeDoesNotContain("odd");
            }

            return self.Next(min, max) | 1;
        }

        /// <summary>
        /// Returns a random odd integer number within range (min [inclusive] and max [exclusive]).
        /// </summary>
        public static int RandomOdd(this IRng self, in DiapasonInt range)
        {
            return RandomOdd(self, range.Min, range.Max);
        }

        /// <summary>
        /// Returns a random odd integer number within range (min [inclusive] and max [exclusive]).
        /// </summary>
        public static int RandomOdd(this IRng self, in (int min, int max) range)
        {
            return RandomOdd(self, range.min, range.max);
        }

        public static string GetAlphanumeric(this IRng self, int length)
        {
            if (length < 0)
                throw ThrowErrors.NegativeParameter(nameof(length));

            if (length == 0)
                return string.Empty;

            const int stackLenCup = 256;
            string symbols = StringUtility.Alphanumeric;

#if UNITY_2021_2_OR_NEWER || !UNITY
            Span<char> charArray = length > stackLenCup ? new char[length] : stackalloc char[length];
            for (int i = 0; i < length; i++) { charArray[i] = getChar(); }
            return new string(charArray);
#else
            if (length > stackLenCup)
            {
                char[] charArray = new char[length];
                for (int i = 0; i < length; i++) { charArray[i] = getChar(); }
                return new string(charArray);
            }

            unsafe
            {
                char* charArray = stackalloc char[length];
                for (int i = 0; i < length; i++) { charArray[i] = getChar(); }
                return new string(charArray, 0, length);
            }
#endif

            char getChar()
            {
                char ch = symbols.GetRandomChar(self);
                return (char.IsDigit(ch) || self.Next(2) == 0) ? ch : char.ToLower(ch);
            }
        }
    }
}
