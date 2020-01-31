using System;
using System.Runtime.CompilerServices;

namespace UU.Collections
{
    public static partial class BitMaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TEnum>(this ref int mask, TEnum flag) where TEnum : Enum
        {
            mask.AddFlag(flag.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove<TEnum>(this ref int mask, TEnum flag) where TEnum : Enum
        {
            mask.RemoveFlag(flag.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Switch<TEnum>(this ref int mask, TEnum flag) where TEnum : Enum
        {
            mask.SwitchFlag(flag.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TEnum>(this in int mask, TEnum flag) where TEnum : Enum
        {
            return mask.ContainsFlag(flag.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TEnum>(this ref int mask, TEnum flag, bool value) where TEnum : Enum
        {
            mask.SetFlag(flag.ToInteger(), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAll<TEnum>(this ref int mask) where TEnum : Enum
        {
            mask.AddAll(Enum<TEnum>.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAll<TEnum>(this ref int mask, bool value) where TEnum : Enum
        {
            mask.SetAll(value, Enum<TEnum>.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All<TEnum>(this int mask) where TEnum : Enum
        {
            return mask.AllFor(Enum<TEnum>.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<TEnum>(this int mask) where TEnum : Enum
        {
            return mask.AnyFor(Enum<TEnum>.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCount<TEnum>(this int mask) where TEnum : Enum
        {
            return mask.GetCount(Enum<TEnum>.Count);
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TEnum>(this BitArrayMask mask, TEnum flag, bool value) where TEnum : Enum
        {
            mask.Set(flag.ToInteger(), value);
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
