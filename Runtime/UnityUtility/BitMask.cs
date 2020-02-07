using System;
using System.Runtime.CompilerServices;

namespace UU
{
    public static class BitMask
    {
        internal const int SIZE = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(int mask)
        {
            return mask == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty(int mask)
        {
            return mask != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFull(int mask)
        {
            return mask == -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlag(ref int mask, int index)
        {
            mask |= 1 << index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlag(ref int mask, int index)
        {
            mask &= ~(1 << index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwitchFlag(ref int mask, int index)
        {
            mask ^= 1 << index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsFlag(int mask, int index)
        {
            return (mask & 1 << index) != 0;
        }

        public static void SetFlag(ref int mask, int index, bool flagValue)
        {
            if (flagValue) { AddFlag(ref mask, index); }
            else { RemoveFlag(ref mask, index); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlags(ref int mask, int otherMask)
        {
            mask |= otherMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlags(ref int mask, int otherMask)
        {
            mask ^= mask & otherMask;
        }

        public static void SetFlags(ref int mask, int otherMask, bool flagValue)
        {
            if (flagValue) { AddFlags(ref mask, otherMask); }
            else { RemoveFlags(ref mask, otherMask); }
        }

        public static void AddAll(ref int mask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
            {
                mask = -1;
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    AddFlag(ref mask, i);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(ref int mask)
        {
            mask = 0;
        }

        public static void SetAll(ref int mask, bool flagValue, int length = SIZE)
        {
            if (flagValue) { AddAll(ref mask, length); }
            else { Clear(ref mask); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvertAll(ref int mask)
        {
            mask = ~mask;
        }

        public static bool AllOf(int mask, int otherMask)
        {
            for (int i = 0; i < SIZE; i++)
            {
                if (ContainsFlag(otherMask, i) && !ContainsFlag(mask, i))
                    return false;
            }

            return true;
        }

        public static bool AnyOf(int mask, int otherMask)
        {
            for (int i = 0; i < SIZE; i++)
            {
                if (ContainsFlag(otherMask, i) && ContainsFlag(mask, i))
                    return true;
            }

            return false;
        }

        public static bool AllFor(int mask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
                return IsFull(mask);

            for (int i = 0; i < length; i++)
            {
                if (!ContainsFlag(mask, i))
                    return false;
            }
            return true;
        }

        public static bool AnyFor(int mask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
                return IsNotEmpty(mask);

            for (int i = 0; i < length; i++)
            {
                if (ContainsFlag(mask, i))
                    return true;
            }
            return false;
        }

        public static int GetCount(int mask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
                return GetUnitsCount(mask);

            int count = 0;
            for (int i = 0; i < length; i++)
            {
                if (ContainsFlag(mask, i))
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
