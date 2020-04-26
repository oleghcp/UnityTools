using System;
using UnityUtility.Collections.Unsafe;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityUtility.Collections;
using UnityUtility.MathExt;
using UnityUtility.Rng;
using UnityEngine;

using UR = UnityEngine.Random;
using UnityUtility.BitMasks;

namespace UnityUtility
{
    /// <summary>
    /// Class for generating random data.
    /// </summary>
    public static class Rnd
    {
        private static IRng s_rng;

        static Rnd()
        {
            s_rng = new UnityRNG();
        }

        public static void OverrideRandomizer(IRng randomizer)
        {
            s_rng = randomizer;
        }

        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int Random(int min, int max)
        {
            return s_rng.Next(min, max);
        }

        /// <summary>
        /// Returns a random integer number between zero [inclusive] and max [exclusive].
        /// </summary>
        public static int Random(int max)
        {
            return s_rng.Next(max);
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive].
        /// </summary>
        public static float Random(float min, float max)
        {
            return s_rng.NextFloat(min, max);
        }

        /// <summary>
        /// Returns a random float number between zero [inclusive] and max [inclusive].
        /// </summary>
        public static float Random(float max)
        {
            return s_rng.NextFloat(0f, max);
        }

        /// <summary>
        /// Returns a random float number between -range [inclusive] and range [inclusive].
        /// </summary>
        public static float Range(float range)
        {
            return s_rng.NextFloat(-range, range);
        }

        /// <summary>
        /// Returns true with chance from 0f to 1f.
        /// </summary>
        public static bool Chance(float chance)
        {
            return chance > s_rng.NextDouble();
        }

        /// <summary>
        /// Returns true with chance represented by Percent from 0 to 100.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chance(Percent chance)
        {
            return Chance(chance.ToRatio());
        }

        //public static int RandomTemp(float[] weights)
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
        public static int Random(float[] weights)
        {
            double rnd = s_rng.NextDouble() * weights.Sum();
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
        public static unsafe int Random(float* weights, int length)
        {
            double rnd = s_rng.NextDouble() * ArrayUtility.Sum(weights, length);
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
        public static int Random(int[] weights)
        {
            int rnd = s_rng.Next(weights.Sum());
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
        public static unsafe int Random(int* weights, int length)
        {
            int rnd = s_rng.Next(ArrayUtility.Sum(weights, length));
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
        public static int RandomFlag(int mask, int length)
        {
            int rn = Random(mask.GetCount(length));

            for (int i = 0; i < length; i++)
            {
                if (mask.ContainsFlag(i))
                {
                    if (rn-- == 0)
                        return i;
                }
            }

            throw new InvalidOperationException("Mask is empty.");
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask.
        /// </summary>
        public static int RandomFlag(BitArrayMask mask)
        {
            int rn = Random(mask.GetCount());

            for (int i = 0; i < mask.Length; i++)
            {
                if (mask.Get(i))
                {
                    if (rn-- == 0)
                        return i;
                }
            }

            throw new InvalidOperationException("Mask is empty.");
        }

        #region randoms by condition
        //TODO: need check of impossible condition

        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive] and which is not equal to exclusiveValue.
        /// </summary>
        public static int Random(int min, int max, int exclusiveValue)
        {
            int value;
            do { value = s_rng.Next(min, max); } while (value == exclusiveValue);
            return value;
        }

        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive] and which is satisfies the specified condition.
        /// </summary>
        public static int Random(int min, int max, Func<int, bool> condition)
        {
            int value;
            do { value = s_rng.Next(min, max); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] and which is satisfies the specified condition.
        /// </summary>
        public static float Random(float min, float max, Func<float, bool> condition)
        {
            float value;
            do { value = s_rng.NextFloat(min, max); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask and which is satisfies the specified condition.
        /// </summary>
        public static int RandomFlag(int mask, int length, Func<int, bool> condition)
        {
            int value;
            do { value = RandomFlag(mask, length); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask and which is satisfies the specified condition.
        /// </summary>
        public static int RandomFlag(BitArrayMask mask, Func<int, bool> condition)
        {
            int value;
            do { value = RandomFlag(mask); } while (!condition(value));
            return value;
        }
        #endregion

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min values.
        /// </summary>
        public static float Descending(float min, float max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), $"{nameof(min)} cannot be more than {nameof(max)}.");

            float range = max - min;
            float rnd = s_rng.NextFloat(0f, 1f);
            return rnd * rnd * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to max values.
        /// </summary>
        public static float Ascending(float min, float max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), $"{nameof(min)} cannot be more than {nameof(max)}.");

            float range = max - min;
            float rnd = s_rng.NextFloat(0f, 1f);
            return rnd.Sqrt() * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min values if curvature &gt; 1f or to max values if curvature &lt; 1f.
        /// </summary>
        /// <param name="curvature">Power of the offset dependency (cannot be negative). If the value is 1f the function has no chance offset because of linear dependency.</param>
        public static float Side(float min, float max, float curvature)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), $"{nameof(min)} cannot be more than {nameof(max)}.");

            if (curvature < 0f)
                throw new ArgumentOutOfRangeException(nameof(curvature), nameof(curvature) + " cannot be negative.");

            float range = max - min;
            float rnd = s_rng.NextFloat(0f, 1f);
            return rnd.Pow(curvature) * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min and max values if curvature &gt; 1f or to average values if curvature &lt; 1f.
        /// </summary>
        /// <param name="curvature">Power of the offset dependency (cannot be negative). If the value is 1f the function has no chance offset because of linear dependency.</param>
        public static float Symmetric(float min, float max, float curvature)
        {
            float average = (max + min) * 0.5f;

            if (s_rng.Next(0, 2) == 0)
                return Side(min, average, curvature);
            else
                return Side(average, max, 1f / curvature);
        }

        /// <summary>
        /// Returns a random even integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int RandomEven(int min, int max)
        {
            return s_rng.Next(min, max) & -2;
        }

        /// <summary>
        /// Returns a random odd integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int RandomOdd(int min, int max)
        {
            return s_rng.Next(min, max - 1) | 1; //TODO: need check for (max - 1).
        }

        /// <summary>
        /// Fills a byte array with random values.
        /// </summary>
        public static void RandomByteArray(byte[] buffer)
        {
            s_rng.NextBytes(buffer);
        }

        /// <summary>
        /// Fills a byte array with random values.
        /// </summary>
        public static unsafe void RandomByteArray(byte* arrayPtr, int length)
        {
            s_rng.NextBytes(arrayPtr, length);
        }

        /// <summary>
        /// Returns a random point inside a circle with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetInsideUnitCircle()
        {
            return UR.insideUnitCircle;
        }

        /// <summary>
        /// Returns a random point on the circle line with radius 1.
        /// </summary>
        public static Vector2 GetOnUnitCircle()
        {
            double angle = s_rng.NextDouble() * Math.PI * 2d;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Returns a random point inside a sphere with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetInsideUnitSphere()
        {
            return UR.insideUnitSphere;
        }

        /// <summary>
        /// Returns a random point on the surface of a sphere with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetOnUnitSphere()
        {
            return UR.onUnitSphere;
        }

        /// <summary>
        /// Returns a random rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion GetRandomRot(bool uniformDistribution = false)
        {
            return uniformDistribution ? UR.rotationUniform : UR.rotation;
        }

        /// <summary>
        /// Returns a random color32 with the specified alfa.
        /// </summary>
        public static Color32 GetRandomColor(byte alfa)
        {
            Bytes bytes = default;
            int channel1 = s_rng.Next(3);
            int channel2 = Random(0, 3, channel1);
            bytes[channel1] = byte.MaxValue;
            bytes[channel2] = s_rng.NextByte();
            bytes[3] = alfa;
            return (Color32)bytes;
        }

        /// <summary>
        /// Returns a random color32 with random alfa.
        /// </summary>
        public static Color32 GetRandomColor()
        {
            return GetRandomColor(s_rng.NextByte());
        }
    }
}
