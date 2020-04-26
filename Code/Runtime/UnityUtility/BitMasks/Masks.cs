using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityUtility.Collections;

namespace UnityUtility.BitMasks
{
    public static class Masks
    {
        public static int CreateMask(int index0)
        {
            int mask = 0;
            BitMask.AddFlag(ref mask, index0);
            return mask;
        }

        public static int CreateMask(int index0, int index1)
        {
            int mask = 0;
            BitMask.AddFlag(ref mask, index0);
            BitMask.AddFlag(ref mask, index1);
            return mask;
        }

        public static int CreateMask(int index0, int index1, int index2)
        {
            int mask = 0;
            BitMask.AddFlag(ref mask, index0);
            BitMask.AddFlag(ref mask, index1);
            BitMask.AddFlag(ref mask, index2);
            return mask;
        }

        public static int CreateMask(int index0, int index1, int index2, int index3)
        {
            int mask = 0;
            BitMask.AddFlag(ref mask, index0);
            BitMask.AddFlag(ref mask, index1);
            BitMask.AddFlag(ref mask, index2);
            BitMask.AddFlag(ref mask, index3);
            return mask;
        }

        public static int CreateMask(params int[] indices)
        {
            int mask = 0;
            for (int i = 0; i < indices.Length; i++)
            { BitMask.AddFlag(ref mask, indices[i]); }
            return mask;
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(TEnum index0) where TEnum : Enum
        {
            return CreateMask(index0.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(TEnum index0, TEnum index1) where TEnum : Enum
        {
            return CreateMask(index0.ToInteger(), index1.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(TEnum index0, TEnum index1, TEnum index2) where TEnum : Enum
        {
            return CreateMask(index0.ToInteger(), index1.ToInteger(), index2.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(TEnum index0, TEnum index1, TEnum index2, TEnum index3) where TEnum : Enum
        {
            return CreateMask(index0.ToInteger(), index1.ToInteger(), index2.ToInteger(), index3.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask<TEnum>(params TEnum[] indices) where TEnum : Enum
        {
            return CreateMask(indices.GetConverted(itm => itm.ToInteger()));
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(TEnum index0) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, index0.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(TEnum index0, TEnum index1) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, index0.ToInteger(), index1.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(TEnum index0, TEnum index1, TEnum index2) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, index0.ToInteger(), index1.ToInteger(), index2.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(TEnum index0, TEnum index1, TEnum index2, TEnum index3) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, index0.ToInteger(), index1.ToInteger(), index2.ToInteger(), index3.ToInteger());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArrayMask CreateArray<TEnum>(params TEnum[] flags) where TEnum : Enum
        {
            return new BitArrayMask(Enum<TEnum>.Count, flags.GetConverted(itm => itm.ToInteger()));
        }
    }
}
