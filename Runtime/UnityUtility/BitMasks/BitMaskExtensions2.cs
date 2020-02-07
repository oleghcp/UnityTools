using System;
using System.Runtime.CompilerServices;
using UU.Collections;

namespace UU.BitMasks
{
    public static partial class BitMaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TEnum>(this ref int mask, TEnum flag) where TEnum : Enum
        {
            BitMask.AddFlag(ref mask, flag.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove<TEnum>(this ref int mask, TEnum flag) where TEnum : Enum
        {
            BitMask.RemoveFlag(ref mask, flag.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Switch<TEnum>(this ref int mask, TEnum flag) where TEnum : Enum
        {
            BitMask.SwitchFlag(ref mask, flag.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TEnum>(this in int mask, TEnum flag) where TEnum : Enum
        {
            return BitMask.ContainsFlag(mask, flag.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TEnum>(this ref int mask, TEnum flag, bool flagValue) where TEnum : Enum
        {
            BitMask.SetFlag(ref mask, flag.ToInteger(), flagValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAll<TEnum>(this ref int mask) where TEnum : Enum
        {
            BitMask.AddAll(ref mask, Enum<TEnum>.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAll<TEnum>(this ref int mask, bool flagValue) where TEnum : Enum
        {
            BitMask.SetAll(ref mask, flagValue, Enum<TEnum>.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All<TEnum>(this int mask) where TEnum : Enum
        {
            return BitMask.AllFor(mask, Enum<TEnum>.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<TEnum>(this int mask) where TEnum : Enum
        {
            return BitMask.AnyFor(mask, Enum<TEnum>.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCount<TEnum>(this int mask) where TEnum : Enum
        {
            return BitMask.GetCount(mask, Enum<TEnum>.Count);
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TEnum>(this BitArrayMask mask, TEnum flag, bool flagValue) where TEnum : Enum
        {
            mask.Set(flag.ToInteger(), flagValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Get<TEnum>(this BitArrayMask mask, TEnum flag) where TEnum : Enum
        {
            return mask.Get(flag.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TEnum>(this BitArrayMask mask, TEnum flag) where TEnum : Enum
        {
            mask.Set(flag.ToInteger(), true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove<TEnum>(this BitArrayMask mask, TEnum flag) where TEnum : Enum
        {
            mask.Set(flag.ToInteger(), false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Switch<TEnum>(this BitArrayMask mask, TEnum flag) where TEnum : Enum
        {
            int index = flag.ToInteger();
            mask.Set(index, !mask.Get(index));
        }
    }
}
