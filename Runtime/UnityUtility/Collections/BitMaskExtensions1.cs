using System;
using System.Runtime.CompilerServices;

namespace UU.Collections
{
    public static partial class BitMaskExtensions
    {
        internal const int SIZE = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this int value)
        {
            return value == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty(this int value)
        {
            return value != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFull(this int value)
        {
            return value == -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlag(this ref int value, int index)
        {
            value |= 1 << index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlag(this ref int value, int index)
        {
            value &= ~(1 << index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwitchFlag(this ref int value, int index)
        {
            value ^= 1 << index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsFlag(this int value, int index)
        {
            return (value & 1 << index) != 0;
        }

        public static void SetFlag(this ref int value, int index, bool flagValue)
        {
            if (flagValue) { value.AddFlag(index); }
            else { value.RemoveFlag(index); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlags(this ref int value, int otherFlags)
        {
            value |= otherFlags;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlags(this ref int value, int otherFlags)
        {
            value ^= value & otherFlags;
        }

        public static void SetFlags(this ref int value, int otherFlags, bool flagValue)
        {
            if (flagValue) { value.AddFlags(otherFlags); }
            else { value.RemoveFlags(otherFlags); }
        }

        public static void AddAll(this ref int value, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
            {
                value = -1;
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    value.AddFlag(i);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(this ref int value)
        {
            value = 0;
        }

        public static void SetAll(this ref int value, bool flagValue, int length = SIZE)
        {
            if (flagValue) { value.AddAll(length); }
            else { value.Clear(); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvertAll(this ref int value)
        {
            value = ~value;
        }

        public static bool AllOf(this int value, int otherFlags)
        {
            for (int i = 0; i < SIZE; i++)
            {
                if (otherFlags.ContainsFlag(i) && !value.ContainsFlag(i))
                    return false;
            }

            return true;
        }

        public static bool AnyOf(this int value, int otherFlags)
        {
            for (int i = 0; i < SIZE; i++)
            {
                if (otherFlags.ContainsFlag(i) && value.ContainsFlag(i))
                    return true;
            }

            return false;
        }

        public static bool AllFor(this int value, int length)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
                return value.IsFull();

            for (int i = 0; i < length; i++)
            {
                if (!value.ContainsFlag(i))
                    return false;
            }
            return true;
        }

        public static bool AnyFor(this int value, int length)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
                return value.IsNotEmpty();

            for (int i = 0; i < length; i++)
            {
                if (value.ContainsFlag(i))
                    return true;
            }
            return false;
        }

        public static int GetCount(this int value, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
                return GetUnitsCount(value);

            int count = 0;
            for (int i = 0; i < length; i++)
            {
                if (value.ContainsFlag(i))
                    count++;
            }
            return count;
        }

        internal static int GetUnitsCount(int mask)
        {
            mask -= (mask >> 1) & 0x55555555;
            mask = ((mask >> 2) & 0x33333333) + (mask & 0x33333333);
            mask = ((((mask >> 4) + mask) & 0x0F0F0F0F) * 0x01010101) >> 24;
            return mask;
        }
    }
}
