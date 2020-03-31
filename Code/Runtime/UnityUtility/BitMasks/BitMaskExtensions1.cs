using System.Runtime.CompilerServices;

namespace UnityUtility.BitMasks
{
    public static partial class BitMaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this int mask)
        {
            return BitMask.IsEmpty(mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty(this int mask)
        {
            return BitMask.IsNotEmpty(mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFull(this int mask)
        {
            return BitMask.IsFull(mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlag(this ref int mask, int index)
        {
            BitMask.AddFlag(ref mask, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlag(this ref int mask, int index)
        {
            BitMask.RemoveFlag(ref mask, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwitchFlag(this ref int mask, int index)
        {
            BitMask.SwitchFlag(ref mask, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsFlag(this int mask, int index)
        {
            return BitMask.ContainsFlag(mask, index);
        }

        public static void SetFlag(this ref int mask, int index, bool flagValue)
        {
            BitMask.SetFlag(ref mask, index, flagValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlags(this ref int mask, int otherMask)
        {
            BitMask.AddFlags(ref mask, otherMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlags(this ref int mask, int otherMask)
        {
            BitMask.RemoveFlags(ref mask, otherMask);
        }

        public static void SetFlags(this ref int mask, int otherMask, bool flagValue)
        {
            BitMask.SetFlags(ref mask, otherMask, flagValue);
        }

        public static void AddAll(this ref int mask, int length = BitMask.SIZE)
        {
            BitMask.AddAll(ref mask, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(this ref int mask)
        {
            BitMask.Clear(ref mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAll(this ref int mask, bool flagValue, int length = BitMask.SIZE)
        {
            BitMask.SetAll(ref mask, flagValue, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvertAll(this ref int mask)
        {
            BitMask.InvertAll(ref mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllOf(this int mask, int otherMask)
        {
            return BitMask.AllOf(mask, otherMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyOf(this int mask, int otherMask)
        {
            return BitMask.AnyOf(mask, otherMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllFor(this int mask, int length = BitMask.SIZE)
        {
            return BitMask.AllFor(mask, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyFor(this int mask, int length = BitMask.SIZE)
        {
            return BitMask.AnyFor(mask, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCount(this int mask, int length = BitMask.SIZE)
        {
            return BitMask.GetCount(mask, length);
        }
    }
}
