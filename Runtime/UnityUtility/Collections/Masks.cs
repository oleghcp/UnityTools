using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UU.Collections
{
    public static class Masks
    {
        public static int CreateMask(int flag0)
        {
            int mask = 0;
            mask.AddFlag(flag0);
            return mask;
        }

        public static int CreateMask(int flag0, int flag1)
        {
            int mask = 0;
            mask.AddFlag(flag0);
            mask.AddFlag(flag1);
            return mask;
        }

        public static int CreateMask(int flag0, int flag1, int flag2)
        {
            int mask = 0;
            mask.AddFlag(flag0);
            mask.AddFlag(flag1);
            mask.AddFlag(flag2);
            return mask;
        }

        public static int CreateMask(int flag0, int flag1, int flag2, int flag3)
        {
            int mask = 0;
            mask.AddFlag(flag0);
            mask.AddFlag(flag1);
            mask.AddFlag(flag2);
            mask.AddFlag(flag3);
            return mask;
        }

        public static int CreateMask(params int[] flags)
        {
            int mask = 0;
            for (int i = 0; i < flags.Length; i++)
            { mask.AddFlag(flags[i]); }
            return mask;
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(TEnum flag0) where TEnum : Enum
        {
            return CreateMask(flag0.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(TEnum flag0, TEnum flag1) where TEnum : Enum
        {
            return CreateMask(flag0.ToInteger(), flag1.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(TEnum flag0, TEnum flag1, TEnum flag2) where TEnum : Enum
        {
            return CreateMask(flag0.ToInteger(), flag1.ToInteger(), flag2.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(TEnum flag0, TEnum flag1, TEnum flag2, TEnum flag3) where TEnum : Enum
        {
            return CreateMask(flag0.ToInteger(), flag1.ToInteger(), flag2.ToInteger(), flag3.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(params TEnum[] flags) where TEnum : Enum
        {
            return CreateMask(flags.GetConverted(itm => itm.ToInteger()));
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(TEnum flag0) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, flag0.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(TEnum flag0, TEnum flag1) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, flag0.ToInteger(), flag1.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(TEnum flag0, TEnum flag1, TEnum flag2) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, flag0.ToInteger(), flag1.ToInteger(), flag2.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(TEnum flag0, TEnum flag1, TEnum flag2, TEnum flag3) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, flag0.ToInteger(), flag1.ToInteger(), flag2.ToInteger(), flag3.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(params TEnum[] flags) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, flags.GetConverted(itm => itm.ToInteger()));
        }
    }
}
