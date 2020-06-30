using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Tools;
using UnityEngine;
using UnityUtility.BitMasks;
using UnityUtility.Collections;
using UnityUtility.Collections.Unsafe;
using UnityUtility.MathExt;
using UnityUtility.Rng;

namespace UnityUtility
{
    public interface IRng
    {
        int Next(int minValue, int maxValue);
        int Next(int maxValue);
        float NextFloat(float minValue, float maxValue);
        float NextFloat(float maxValue);
        double NextDouble();
        byte NextByte();
        void NextBytes(byte[] buffer);
        unsafe void NextBytes(byte* arrayPtr, int length);
    }

    /// <summary>
    /// Class for generating random data.
    /// </summary>
    public static class RngExtensions
    {
        /// <summary>
        /// Returns a random float number between -range [inclusive] and range [inclusive].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Range(this IRng self, float range)
        {
            return self.NextFloat(-range, range);
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
        /// Returns true with chance represented by Percent from 0 to 100.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chance(this IRng self, Percent chance)
        {
            return chance.ToRatio() > self.NextDouble();
        }

        //public static  int RandomTemp(float[] weights)
        //{
        //    float sum = weights.Sum();
        //    float factor = 1f / sum;

        //    for (int i = 0; i < weights.Length; i++)
        //    {
        //        if (Chance(weights[i] * factor))
        //            return i;

        //        sum -= weights[i];
        //        factor = 1f / sum;
        //    }

        //    return -1;
        //}

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 for none of the elements (if all weights are zero). Each element cannot be less than 0f.
        /// </summary>
        public static int Random(this IRng self, float[] weights)
        {
            double rnd = self.NextDouble() * weights.Sum();
            double sum = 0d;

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] + sum > rnd)
                    return i;

                sum += weights[i];
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 for none of the elements (if all weights are zero). Each element cannot be less than 0f.
        /// </summary>
        public static unsafe int Random(this IRng self, float* weights, int length)
        {
            double rnd = self.NextDouble() * ArrayUtility.Sum(weights, length);
            double sum = 0d;

            for (int i = 0; i < length; i++)
            {
                if (weights[i] + sum > rnd)
                    return i;

                sum += weights[i];
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 for none of the elements (if all weights are zero). Each element cannot be less than 0f.
        /// </summary>
        public static int Random(this IRng self, int[] weights)
        {
            int rnd = self.Next(weights.Sum());
            int sum = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] + sum > rnd)
                    return i;

                sum += weights[i];
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 for none of the elements (if all weights are zero). Each element cannot be less than 0f.
        /// </summary>
        public static unsafe int Random(this IRng self, int* weights, int length)
        {
            int rnd = self.Next(ArrayUtility.Sum(weights, length));
            int sum = 0;

            for (int i = 0; i < length; i++)
            {
                if (weights[i] + sum > rnd)
                    return i;

                sum += weights[i];
            }

            return -1;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask.
        /// </summary>
        /// <param name="length">How many flags of 32bit mask should be considered.</param>
        public static int RandomFlag(this IRng self, int mask, int length)
        {
            int rn = self.Next(mask.GetCount(length));

            for (int i = 0; i < length; i++)
            {
                if (mask.ContainsFlag(i))
                {
                    if (rn-- == 0)
                        return i;
                }
            }

            throw Errors.EmptyMask();
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask.
        /// </summary>
        public static int RandomFlag(this IRng self, BitArrayMask mask)
        {
            int rn = self.Next(mask.GetCount());

            for (int i = 0; i < mask.Length; i++)
            {
                if (mask.Get(i))
                {
                    if (rn-- == 0)
                        return i;
                }
            }

            throw Errors.EmptyMask();
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
            do { value = self.NextFloat(min, max); } while (!condition(value));
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
        public static int RandomFlag(this IRng self, BitArrayMask mask, Func<int, bool> condition)
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
            float rnd = self.NextFloat(0f, 1f);
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
            float rnd = self.NextFloat(0f, 1f);
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
            float rnd = self.NextFloat(0f, 1f);
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
        /// Fills a byte array with random values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomByteArray(this IRng self, byte[] buffer)
        {
            self.NextBytes(buffer);
        }

        /// <summary>
        /// Fills a byte array with random values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void RandomByteArray(this IRng self, byte* arrayPtr, int length)
        {
            self.NextBytes(arrayPtr, length);
        }

        /// <summary>
        /// Returns a random point inside a circle with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetInsideUnitCircle(this IRng _)
        {
            return UnityRng.GetInsideUnitCircle();
        }

        /// <summary>
        /// Returns a random point on the circle line with radius 1.
        /// </summary>
        public static Vector2 GetOnUnitCircle(this IRng self)
        {
            double angle = self.NextDouble() * Math.PI * 2d;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Returns a random point inside a sphere with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetInsideUnitSphere(this IRng _)
        {
            return UnityRng.GetInsideUnitSphere();
        }

        /// <summary>
        /// Returns a random point on the surface of a sphere with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetOnUnitSphere(this IRng _)
        {
            return UnityRng.GetOnUnitSphere();
        }

        /// <summary>
        /// Returns a random rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion GetRandomRot(this IRng _, bool uniformDistribution = false)
        {
            return UnityRng.GetRandomRot(uniformDistribution);
        }

        /// <summary>
        /// Returns a random color32 with the specified alfa.
        /// </summary>
        public static Color32 GetRandomColor(this IRng self, byte alfa)
        {
            Bytes bytes = default;
            int channel1 = self.Next(3);
            int channel2 = self.Random(0, 3, channel1);
            bytes[channel1] = byte.MaxValue;
            bytes[channel2] = self.NextByte();
            bytes[3] = alfa;
            return (Color32)bytes;
        }

        /// <summary>
        /// Returns a random color32 with random alfa.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 GetRandomColor(this IRng self)
        {
            return self.GetRandomColor(self.NextByte());
        }

        /// <summary>
        /// Shuffles the elements of an entire collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this IRng self, IList<T> collection)
        {
            CollectionHelper.Shuffle(collection, self);
        }
    }
}
