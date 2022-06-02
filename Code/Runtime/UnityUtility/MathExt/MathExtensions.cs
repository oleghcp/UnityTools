using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityUtility.MathExt
{
    public enum RoundingWay
    {
        ToNearest,
        Ceiling,
        Floor,
    }

    public static class MathExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to negative or positive infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinity(this float value)
        {
            return float.IsInfinity(value);
        }

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to negative infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegInfinity(this float value)
        {
            return float.IsNegativeInfinity(value);
        }

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to positive infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPosInfinity(this float value)
        {
            return float.IsPositiveInfinity(value);
        }

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to not a number.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(this float value)
        {
            return float.IsNaN(value);
        }

        /// <summary>
        /// Compares two big floating point values if they are similar.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approx(this float value, float other)
        {
            return Mathf.Approximately(value, other);
        }

        /// <summary>
        /// Counts digits amount.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Digits(this int number)
        {
            return (number != 0) ? (int)Math.Ceiling(Math.Log10(Math.Abs(number) + 0.5d)) : 1;
        }

        /// <summary>
        /// Returns the sign of value (1, -1 or 0).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sign0(this float value)
        {
            return Math.Sign(value);
        }

        /// <summary>
        /// Returns the sign of value (1, -1 or 0).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign0(this int value)
        {
            return Math.Sign(value);
        }

        /// <summary>
        /// Returns the sign of value (1 of -1).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sign(this float value)
        {
            return Mathf.Sign(value);
        }

        /// <summary>
        /// Returns the sign of value (1 of -1).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this int value)
        {
            return (int)Mathf.Sign(value);
        }

        /// <summary>
        /// Returns true if the value is in range between min [inclusive] and max [inclusive].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInBounds(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Returns true if the value is in range between min [inclusive] and max [exclusive].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInBounds(this int value, int min, int max)
        {
            return value >= min && value < max;
        }

        /// <summary>
        /// Rounds the float value to the nearest integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this float value)
        {
            //return (int)Math.Round(value, MidpointRounding.AwayFromZero);
            return (int)(value + 0.5f * value.Sign());
        }

        /// <summary>
        /// Rounds the float value to the nearest value multiple to the specified step.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(this float value, float snapStep)
        {
            return (value / snapStep).Round() * snapStep;
        }

        /// <summary>
        /// Returns the smallest integer greater to or equal to the value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ceiling(this float value)
        {
            return (int)MathF.Ceiling(value);
        }

        /// <summary>
        /// Returns the largest integer smaller to or equal to the value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Floor(this float value)
        {
            return (int)MathF.Floor(value);
        }

        /// <summary>
        /// Returns the whole part of the specified value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Truncate(this float value)
        {
            return MathF.Truncate(value);
        }

        /// <summary>
        /// Returns the decimal part of the specified value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Decim(this float value)
        {
            return value - (int)value;
        }

        /// <summary>
        /// Returns the absolute value of the number.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(this float value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Returns the absolute value of the number.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Raises the value to the specified power.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(this float value, float pow)
        {
            return MathF.Pow(value, pow);
        }

        /// <summary>
        /// Raises the value to the specified power.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(this int value, float pow)
        {
            return MathF.Pow(value, pow);
        }

        /// <summary>
        /// Returns square root of the value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(this float value)
        {
            return MathF.Sqrt(value);
        }

        /// <summary>
        /// Returns cubic root of the value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cbrt(this float value)
        {
            return MathF.Cbrt(value);
        }

        /// <summary>
        /// Returns square root of the value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(this int value)
        {
            return MathF.Sqrt(value);
        }

        /// <summary>
        /// Returns cubic root of the value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cbrt(this int value)
        {
            return MathF.Cbrt(value);
        }

        /// <summary>
        /// Calculates the linear parameter t that produces the interpolant value within the range [a, b] (InverseLerp).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ratio(this float value, float a, float b)
        {
            return Mathf.InverseLerp(a, b, value);
        }

        /// <summary>
        /// Calculates the linear parameter t that produces the interpolant value within the range [a, b] (InverseLerp).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ratio(this int value, int a, int b)
        {
            return Mathf.InverseLerp(a, b, value);
        }

        /// <summary>
        /// Clamps the value between a minimum float and maximum float value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Clamps the value between a minimum float and maximum float value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float value, in (float min, float max) range)
        {
            return Mathf.Clamp(value, range.min, range.max);
        }

        /// <summary>
        /// Clamps the value between the specified minimum float value and float.PositiveInfinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CutBefore(this float value, float min)
        {
            return value < min ? min : value;
        }

        /// <summary>
        /// Clamps the value between float.NegativeInfinity and the specified maximum float value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CutAfter(this float value, float max)
        {
            return value > max ? max : value;
        }

        /// <summary>        
        /// Clamps the value between 0 and 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(this float value)
        {
            return Mathf.Clamp01(value);
        }

        /// <summary>
        /// Clamps the value between a minimum int and maximum int value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int value, int min, int max)
        {
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Clamps the value between the specified minimum int value and int.MaxValue.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CutBefore(this int value, int min)
        {
            return value < min ? min : value;
        }

        /// <summary>
        /// Clamps the value between int.MinValue and the specified maximum int value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CutAfter(this int value, int max)
        {
            return value > max ? max : value;
        }

        /// <summary>
        /// Loops the value t, so that it is never larger than length and never smaller than 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Repeat(this float value, float length)
        {
            return Mathf.Repeat(value, length);
        }

        /// <summary>
        /// Loops the value t, so that it is never larger than length and never smaller than 0.
        /// </summary>
        public static int Repeat(this int value, int length)
        {
            int res = value % length;
            return res >= 0 ? res : res + length;
        }

        /// <summary>
        /// PingPongs the value t, so that it is never larger than length and never smaller than 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PingPong(this float value, float length)
        {
            return Mathf.PingPong(value, length);
        }

        /// <summary>
        /// PingPongs the value t, so that it is never larger than length and never smaller than 0.
        /// </summary>
        public static int PingPong(this int value, int length)
        {
            value = Repeat(value, length * 2);
            return length - Math.Abs(value - length);
        }

        /// <summary>
        /// Transfers the value from radians to degrees.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(this float value)
        {
            return value * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Transfers the value from degrees to radians.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(this float value)
        {
            return value * Mathf.Deg2Rad;
        }

        /// <summary>
        /// Returns true if the value is even.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(this byte value)
        {
            return value % 2 == 0;
        }

        /// <summary>
        /// Returns true if the value is even.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }

        /// <summary>
        /// Returns true if the value is power of two.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPoT(this int value)
        {
            return Mathf.IsPowerOfTwo(value);
        }

        /// <summary>
        /// Returns the closest power of two value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToClosestPoT(this int value)
        {
            return Mathf.ClosestPowerOfTwo(value);
        }

        /// <summary>
        /// Converts the boolean value to integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// Converts the integer value to boolean.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ToBool(this int value)
        {
            return value > 0;
        }

        /// <summary>
        /// Casts float value to integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this float value, RoundingWay rounding)
        {
            switch (rounding)
            {
                case RoundingWay.ToNearest:
                    return value.Round();

                case RoundingWay.Ceiling:
                    return (int)MathF.Ceiling(value);

                case RoundingWay.Floor:
                    return (int)MathF.Floor(value);

                default:
                    throw new UnsupportedValueException(rounding);
            }
        }

        /// <summary>
        /// Casts integer value to float.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(this int value)
        {
            return value;
        }
    }
}
