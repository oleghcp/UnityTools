using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility
{
    /// <summary>
    /// Class for generating random data.
    /// </summary>
    public static class RngExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Random(this IRng self, in (int minValue, int maxValue) range)
        {
            return self.Next(range.minValue, range.maxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Random(this IRng self, in (float minValue, float maxValue) range)
        {
            return self.Next(range.minValue, range.maxValue);
        }

        /// <summary>
        /// Returns true with chance from 0f to 1f.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chance(this IRng self, float chance)
        {
            return chance > self.NextDouble();
        }

        /// <summary>
        /// Returns random index of collection contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, IList<float> weights, float weightOfNone = 0f)
        {
            float target = self.Next(weights.Sum() + weightOfNone);
            int startIndex = self.Next(weights.Count);
            int count = weights.Count + startIndex;
            float sum = 0f;

            for (int i = startIndex; i < count; i++)
            {
                int index = i % weights.Count;

                if (weights[index] + sum >= target)
                    return index;

                sum += weights[index];
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, float[] weights, int weightOfNone = 0)
        {
            return self.Random(weights as IList<float>, weightOfNone);
        }

        /// <summary>
        /// Returns random index of array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, Span<float> weights, float weightOfNone = 0f)
        {
            float target = self.Next(weights.Sum() + weightOfNone);
            int startIndex = self.Next(weights.Length);
            int count = weights.Length + startIndex;
            float sum = 0f;

            for (int i = 0; i < count; i++)
            {
                int index = i % weights.Length;

                if (weights[index] + sum >= target)
                    return index;

                sum += weights[index];
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of collection contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, IList<int> weights, int weightOfNone = 0)
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

            return -1;
        }

        /// <summary>
        /// Returns random index of array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, int[] weights, int weightOfNone = 0)
        {
            return self.Random(weights as IList<int>, weightOfNone);
        }

        /// <summary>
        /// Returns random index of array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, Span<int> weights, int weightOfNone = 0)
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

            return -1;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask. Returns -1 if mask is empty.
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
        /// Returns a random flag contains in the specified mask. Returns -1 if mask is empty.
        /// </summary>
        public static int RandomFlag(this IRng self, BitList mask)
        {
            int rn = self.Next(mask.GetCount());

            for (int i = 0; i < mask.Count; i++)
            {
                if (mask.Get(i))
                {
                    if (rn-- == 0)
                        return i;
                }
            }

            return -1;
        }

        #region randoms by condition
        //TODO: need check of impossible condition

        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive] and which is not equal to exclusiveValue.
        /// </summary>
        public static int Random(this IRng self, int min, int max, int exclusiveValue)
        {
            int value;
            do { value = self.Next(min, max); } while (value == exclusiveValue);
            return value;
        }

        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive] and which is satisfies the specified condition.
        /// </summary>
        public static int Random(this IRng self, int min, int max, Func<int, bool> condition)
        {
            int value;
            do { value = self.Next(min, max); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] and which is satisfies the specified condition.
        /// </summary>
        public static float Random(this IRng self, float min, float max, Func<float, bool> condition)
        {
            float value;
            do { value = self.Next(min, max); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask and which is satisfies the specified condition.
        /// </summary>
        public static int RandomFlag(this IRng self, int mask, int length, Func<int, bool> condition)
        {
            int value;
            do { value = self.RandomFlag(mask, length); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask and which is satisfies the specified condition.
        /// </summary>
        public static int RandomFlag(this IRng self, BitList mask, Func<int, bool> condition)
        {
            int value;
            do { value = self.RandomFlag(mask); } while (!condition(value));
            return value;
        }
        #endregion

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min values.
        /// </summary>
        public static float Descending(this IRng self, float min, float max)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

            float range = max - min;
            float rnd = self.Next(0f, 1f);
            return rnd * rnd * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to max values.
        /// </summary>
        public static float Ascending(this IRng self, float min, float max)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

            float range = max - min;
            float rnd = self.Next(0f, 1f);
            return rnd.Sqrt() * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min values if curvature &gt; 1f or to max values if curvature &lt; 1f.
        /// </summary>
        /// <param name="curvature">Power of the offset dependency (cannot be negative). If the value is 1f the function has no chance offset because of linear dependency.</param>
        public static float Side(this IRng self, float min, float max, float curvature)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

            if (curvature < 0f)
                throw Errors.NegativeParameter(nameof(curvature));

            float range = max - min;
            float rnd = self.Next(0f, 1f);
            return rnd.Pow(curvature) * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min and max values if curvature &gt; 1f or to average values if curvature &lt; 1f.
        /// </summary>
        /// <param name="curvature">Power of the offset dependency (cannot be negative). If the value is 1f the function has no chance offset because of linear dependency.</param>
        public static float Symmetric(this IRng self, float min, float max, float curvature)
        {
            float average = (max + min) * 0.5f;

            if (self.Next(0, 2) == 0)
                return self.Side(min, average, curvature);
            else
                return self.Side(average, max, 1f / curvature);
        }

        /// <summary>
        /// Returns a random even integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int RandomEven(this IRng self, int min, int max)
        {
            if (!min.IsEven())
            {
                if (max - min < 2)
                    throw Errors.RangeDoesNotContain("even");

                min++;
            }

            return self.Next(min, max) & -2;
        }

        /// <summary>
        /// Returns a random odd integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int RandomOdd(this IRng self, int min, int max)
        {
            if (max.IsEven())
            {
                if (min == max)
                    throw Errors.RangeDoesNotContain("odd");
            }
            else if (max - min < 2)
            {
                throw Errors.RangeDoesNotContain("odd");
            }

            return self.Next(min, max) | 1;
        }

        /// <summary>
        /// Returns a random point on the circle line with radius 1.
        /// </summary>
        public static Vector2 GetOnUnitCircle(this IRng self)
        {
            float angle = self.Next(MathF.PI * 2f);
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        /// <summary>
        /// Returns a random point on the surface of a sphere with radius 1.
        /// </summary>
        public static Vector3 GetOnUnitSphere(this IRng self)
        {
            Vector3 vector;
            float magnitude;

            do
            {
                vector = new Vector3(self.Next(-1f, 1f), self.Next(-1f, 1f), self.Next(-1f, 1f));
                magnitude = vector.magnitude;

            } while (magnitude <= MathUtility.kEpsilon || magnitude > 1f);

            return vector / magnitude;
        }

        /// <summary>
        /// Returns a random rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion GetRandomRotation(this IRng self)
        {
            Vector3 up = self.GetOnUnitCircle();
            Vector3 forward = self.GetOnUnitSphere();

            Vector3 axis = Vector3.Cross(Vector3.forward, forward).normalized;

            if (axis.magnitude > MathUtility.kEpsilon)
            {
                float angle = Vector3.Angle(Vector3.forward, forward);
                up = up.GetRotated(axis, angle);
            }

            return Quaternion.LookRotation(forward, up);
        }

        /// <summary>
        /// Returns a random color32 with the specified alfa.
        /// </summary>
        public static Color GetRandomColor(this IRng self)
        {
            return new Color
            {
                r = self.Next(1f),
                g = self.Next(1f),
                b = self.Next(1f),
                a = 1f,
            };
        }

        /// <summary>
        /// Shuffles the elements of an entire collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this IRng self, IList<T> collection)
        {
            CollectionUtility.Shuffle(collection, self);
        }
    }
}
