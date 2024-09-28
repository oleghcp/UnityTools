using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.MathF;

namespace OlegHcp.Mathematics
{
    public static partial class MathUtility
    {
#if !UNITY_2021_2_OR_NEWER
        internal const double THIRD = 1d / 3d;
#endif

        /// <summary>
        /// Rotates an array cell position.
        /// </summary>
        /// <param name="i">Row.</param>
        /// <param name="j">Column.</param>
        /// <param name="rotations">Defines a rotation angle by multiplying by 90 degrees. If the value is positive returns rotated clockwise.</param>
        public static (int i, int j) RotateCellPos(int i, int j, int rotations)
        {
            //Span<int> sinPtr = stackalloc[] { 0, 1, 0, -1 };
            //Span<int> cosPtr = stackalloc[] { 1, 0, -1, 0 };

            //rotations = rotations.Repeat(4);

            //int sin = sinPtr[rotations];
            //int cos = cosPtr[rotations];

            //int i1 = j * sin + i * cos;
            //int j1 = j * cos - i * sin;

            //return (i1, j1);

            switch (rotations.Repeat(4))
            {
                case 1: return (j, -i);
                case 2: return (-i, -j);
                case 3: return (-j, i);
                default: return (i, j);
            }
        }

        public static (float ch1, float ch2) LerpColorChannels(float ratio)
        {
            ratio = 1f - ratio.Clamp01();
            float ch1 = (ratio * 2f).Clamp01();
            float ch2 = (2f - ratio * 2f).Clamp01();
            return (ch1, ch2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ApproachZero(float value)
        {
            return Exp(-value);
        }

        public static float ApproachOne(float value)
        {
            return 1f - Exp(-value);
        }

        public static float GetCircleArea(float radius)
        {
            return PI * radius * radius;
        }

        public static float GetCircumference(float radius)
        {
            return PI * radius * 2f;
        }

        public static float GetSphereVolume(float radius)
        {
            return PI * radius * radius * radius * 4f / 3f;
        }

        public static float GetSphereSurface(float radius)
        {
            return PI * radius * radius * 4f;
        }

        public static int DigitsCount(int number)
        {
            return (number != 0) ? (int)Math.Ceiling(Math.Log10(Math.Abs(number) + 0.5d)) : 1;
        }

        public static int DigitsCount(long number)
        {
            return (number != 0) ? (int)Math.Ceiling(Math.Log10(Math.Abs(number) + 0.5d)) : 1;
        }

        public static void GetDigits(int number, List<int> buffer)
        {
            number = Math.Abs(number);
            while (number > 0)
            {
                buffer.Add(number % 10);
                number /= 10;
            }
            buffer.Reverse();
        }

        public static void GetDigits(long number, List<int> buffer)
        {
            number = Math.Abs(number);
            while (number > 0)
            {
                buffer.Add((int)(number % 10));
                number /= 10;
            }
            buffer.Reverse();
        }

        public static float Lerp(float min, float max, float ratio)
        {
            return min + (max - min) * ratio.Clamp01();
        }

        public static float LerpUnclamped(float min, float max, float ratio)
        {
            return min + (max - min) * ratio;
        }
    }
}
