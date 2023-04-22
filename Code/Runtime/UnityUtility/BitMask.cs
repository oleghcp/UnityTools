using System;
using System.Collections.Generic;

namespace UnityUtility
{
    public static class BitMask
    {
        public const int SIZE = 32;

        public static bool IsEmpty(int mask)
        {
            return mask == 0;
        }

        public static bool IsNotEmpty(int mask)
        {
            return mask != 0;
        }

        public static bool IsFull(int mask)
        {
            return mask == -1;
        }

        public static void AddFlag(ref int mask, int index)
        {
            mask |= 1 << index;
        }

        public static void RemoveFlag(ref int mask, int index)
        {
            mask &= ~(1 << index);
        }

        public static void SwitchFlag(ref int mask, int index)
        {
            mask ^= 1 << index;
        }

        public static bool HasFlag(int mask, int index)
        {
            return (mask & (1 << index)) != 0;
        }

        public static void SetFlag(ref int mask, int index, bool flagValue)
        {
            if (flagValue) { AddFlag(ref mask, index); }
            else { RemoveFlag(ref mask, index); }
        }

        public static void Except(ref int targetMask, int otherMask)
        {
            targetMask ^= targetMask & otherMask;
        }

        public static void AddFor(ref int mask, int length = SIZE)
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

        public static void SetFor(ref int mask, bool flagValue, int length = SIZE)
        {
            if (flagValue) { AddFor(ref mask, length); }
            else { mask = 0; }
        }

        public static void AndFor(ref int targetMask, int otherMask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
            {
                targetMask &= otherMask;
                return;
            }

            int tmpMask = targetMask & otherMask;
            for (int i = 0; i < length; i++)
            {
                SetFlag(ref targetMask, i, HasFlag(tmpMask, i));
            }
        }

        public static void OrFor(ref int targetMask, int otherMask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
            {
                targetMask |= otherMask;
                return;
            }

            int tmpMask = targetMask | otherMask;
            for (int i = 0; i < length; i++)
            {
                SetFlag(ref targetMask, i, HasFlag(tmpMask, i));
            }
        }

        public static void XorFor(ref int targetMask, int otherMask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
            {
                targetMask ^= otherMask;
                return;
            }

            int tmpMask = targetMask ^ otherMask;
            for (int i = 0; i < length; i++)
            {
                SetFlag(ref targetMask, i, HasFlag(tmpMask, i));
            }
        }

        public static void NotFor(ref int mask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
            {
                mask = ~mask;
                return;
            }

            for (int i = 0; i < length; i++)
            {
                SwitchFlag(ref mask, i);
            }
        }

        public static void ExceptFor(ref int targetMask, int otherMask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
            {
                Except(ref targetMask, otherMask);
                return;
            }

            int tmpMask = targetMask;
            Except(ref tmpMask, otherMask);

            for (int i = 0; i < length; i++)
            {
                SetFlag(ref targetMask, i, HasFlag(tmpMask, i));
            }
        }

        public static bool Equals(int mask1, int mask2, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
                return mask1 == mask2;

            int xorMask = mask1 ^ mask2;

            for (int i = 0; i < SIZE; i++)
            {
                if (HasFlag(xorMask, i))
                    return false;
            }

            return true;
        }

        public static bool Intersects(int mask1, int mask2, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            int intersection = mask1 & mask2;

            if (length == SIZE)
                return intersection != 0;

            for (int i = 0; i < length; i++)
            {
                if (HasFlag(intersection, i))
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
                if (!HasFlag(mask, i))
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
                if (HasFlag(mask, i))
                    return true;
            }

            return false;
        }

        public static bool EmptyFor(int mask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == SIZE)
                return IsEmpty(mask);

            for (int i = 0; i < length; i++)
            {
                if (HasFlag(mask, i))
                    return false;
            }

            return true;
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
                if (HasFlag(mask, i))
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

        // -- //

        public static int CreateMask(int index0)
        {
            int mask = 0;
            AddFlag(ref mask, index0);
            return mask;
        }

        public static int CreateMask(int index0, int index1)
        {
            int mask = 0;
            AddFlag(ref mask, index0);
            AddFlag(ref mask, index1);
            return mask;
        }

        public static int CreateMask(int index0, int index1, int index2)
        {
            int mask = 0;
            AddFlag(ref mask, index0);
            AddFlag(ref mask, index1);
            AddFlag(ref mask, index2);
            return mask;
        }

        public static int CreateMask(int index0, int index1, int index2, int index3)
        {
            int mask = 0;
            AddFlag(ref mask, index0);
            AddFlag(ref mask, index1);
            AddFlag(ref mask, index2);
            AddFlag(ref mask, index3);
            return mask;
        }

        public static int CreateMask(params int[] indices)
        {
            int mask = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                AddFlag(ref mask, indices[i]);
            }
            return mask;
        }

        public static IEnumerable<int> EnumerateIndices(int mask, int length = SIZE)
        {
            if (length > SIZE)
                throw new ArgumentOutOfRangeException(nameof(length));

            for (int i = 0; i < length; i++)
            {
                if (HasFlag(mask, i))
                    yield return i;
            }
        }
    }
}
