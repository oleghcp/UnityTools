using System;
using System.Runtime.CompilerServices;
using UnityUtility.Collections;

namespace UnityUtility.BitMasks
{
    public static partial class BitMaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TEnum>(this ref int mask, TEnum enumIndex) where TEnum : Enum
        {
            BitMask.AddFlag(ref mask, enumIndex.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove<TEnum>(this ref int mask, TEnum enumIndex) where TEnum : Enum
        {
            BitMask.RemoveFlag(ref mask, enumIndex.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Switch<TEnum>(this ref int mask, TEnum enumIndex) where TEnum : Enum
        {
            BitMask.SwitchFlag(ref mask, enumIndex.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TEnum>(this in int mask, TEnum enumIndex) where TEnum : Enum
        {
            return BitMask.ContainsFlag(mask, enumIndex.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TEnum>(this ref int mask, TEnum enumIndex, bool flagValue) where TEnum : Enum
        {
            BitMask.SetFlag(ref mask, enumIndex.ToInteger(), flagValue);
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
        public static void Set<TEnum>(this BitArrayMask mask, TEnum enumIndex, bool flagValue) where TEnum : Enum
        {
            mask.Set(enumIndex.ToInteger(), flagValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Get<TEnum>(this BitArrayMask mask, TEnum enumIndex) where TEnum : Enum
        {
            return mask.Get(enumIndex.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TEnum>(this BitArrayMask mask, TEnum enumIndex) where TEnum : Enum
        {
            mask.Set(enumIndex.ToInteger(), true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove<TEnum>(this BitArrayMask mask, TEnum enumIndex) where TEnum : Enum
        {
            mask.Set(enumIndex.ToInteger(), false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Switch<TEnum>(this BitArrayMask mask, TEnum enumIndex) where TEnum : Enum
        {
            int index = enumIndex.ToInteger();
            mask.Set(index, !mask.Get(index));
        }
    }
}
