﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityUtilityTools;

namespace UnityUtility.Collections
{
    //Based on System.Collections.BitArray
    [Serializable]
    public sealed class BitArrayMask : ICloneable, IReadOnlyList<bool>
    {
        [SerializeField, HideInInspector]
        private int[] m_array;
        [SerializeField, HideInInspector]
        private int m_length;

        private int m_version;

        internal static string ArrayFieldName
        {
            get { return nameof(m_array); }
        }

        internal static string LengthFieldName
        {
            get { return nameof(m_length); }
        }

        public bool this[int index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }

        public int Length
        {
            get { return m_length; }
        }

        public int Version
        {
            get { return m_version; }
        }

        int IReadOnlyCollection<bool>.Count
        {
            get { return m_length; }
        }

        [Preserve]
        private BitArrayMask() { }

        public BitArrayMask(int length, bool defaultValue = false)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            m_array = new int[GetArraySize(length)];
            m_length = length;
            int num = defaultValue ? (-1) : 0;
            for (int i = 0; i < m_array.Length; i++)
            {
                m_array[i] = num;
            }
        }

        #region constructor with flag indices
        public BitArrayMask(int length, int flagIndex0)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            m_array = new int[GetArraySize(length)];
            m_length = length;
            Set(flagIndex0, true);
        }

        public BitArrayMask(int length, int flagIndex0, int flagIndex1)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            m_array = new int[GetArraySize(length)];
            m_length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
        }

        public BitArrayMask(int length, int flagIndex0, int flagIndex1, int flagIndex2)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            m_array = new int[GetArraySize(length)];
            m_length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
            Set(flagIndex2, true);
        }

        public BitArrayMask(int length, int flagIndex0, int flagIndex1, int flagIndex2, int flagIndex3)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            m_array = new int[GetArraySize(length)];
            m_length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
            Set(flagIndex2, true);
            Set(flagIndex3, true);
        }

        public BitArrayMask(int length, params int[] indices)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            m_array = new int[GetArraySize(length)];
            m_length = length;
            for (int i = 0; i < indices.Length; i++)
            {
                Set(indices[i], true);
            }
        }
        #endregion

        public BitArrayMask(IEnumerable<bool> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            m_array = new int[2];

            int i = 0;
            foreach (var item in values)
            {
                if (i >= m_array.Length * BitMask.SIZE)
                    Array.Resize(ref m_array, m_array.Length * 2);

                if (item)
                    m_array[i / BitMask.SIZE] |= 1 << i % BitMask.SIZE;

                i++;
            }

            m_length = i;
        }

        public BitArrayMask(ICollection<bool> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            m_length = values.Count;
            m_array = new int[GetArraySize(m_length)];

            int i = 0;
            foreach (var item in values)
            {

                if (item)
                    m_array[i / BitMask.SIZE] |= 1 << i % BitMask.SIZE;

                i++;
            }
        }

        private BitArrayMask(int[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length > 67108863)
                throw new ArgumentException("Array is too large.", nameof(values));

            m_array = new int[values.Length];
            m_length = values.Length * BitMask.SIZE;
            Array.Copy(values, m_array, values.Length);
        }

        public BitArrayMask(BitArrayMask bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            int arrayLength = GetArraySize(bits.m_length);
            m_array = new int[arrayLength];
            m_length = bits.m_length;
            Array.Copy(bits.m_array, m_array, arrayLength);
            m_version = bits.m_version;
        }

        public void SetLength(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");

            int newArraySize = GetArraySize(value);

            if (newArraySize > m_array.Length || newArraySize + 256 < m_array.Length)
            {
                Array.Resize(ref m_array, newArraySize);
            }

            if (value > m_length)
            {
                int num = GetArraySize(m_length) - 1;
                int num2 = m_length % BitMask.SIZE;
                if (num2 > 0)
                {
                    m_array[num] &= (1 << num2) - 1;
                }
                Array.Clear(m_array, num + 1, newArraySize - num - 1);
            }

            m_length = value;
            m_version++;
        }

        public bool Get(int index)
        {
            if (index >= 0 && index < m_length)
                return (m_array[index / BitMask.SIZE] & 1 << index % BitMask.SIZE) != 0;

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public void Set(int index, bool value)
        {
            if (index >= 0 && index < m_length)
            {
                if (value)
                    m_array[index / BitMask.SIZE] |= 1 << index % BitMask.SIZE;
                else
                    m_array[index / BitMask.SIZE] &= ~(1 << index % BitMask.SIZE);
                m_version++;
                return;
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public void SetAll(bool value)
        {
            int num = value ? (-1) : 0;
            int arrayLength = GetArraySize(m_length);
            for (int i = 0; i < arrayLength; i++)
            {
                m_array[i] = num;
            }
            m_version++;
        }

        public void And(BitArrayMask value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (m_length != value.m_length)
                throw Errors.DifferentArrayLengths();

            int arrayLength = GetArraySize(m_length);
            for (int i = 0; i < arrayLength; i++)
            {
                m_array[i] &= value.m_array[i];
            }
            m_version++;
        }

        public void Or(BitArrayMask value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (m_length != value.m_length)
                throw Errors.DifferentArrayLengths();

            int arrayLength = GetArraySize(m_length);
            for (int i = 0; i < arrayLength; i++)
            {
                m_array[i] |= value.m_array[i];
            }
            m_version++;
        }

        public void Xor(BitArrayMask value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (m_length != value.m_length)
                throw Errors.DifferentArrayLengths();

            int arrayLength = GetArraySize(m_length);
            for (int i = 0; i < arrayLength; i++)
            {
                m_array[i] ^= value.m_array[i];
            }
            m_version++;
        }

        public void Not()
        {
            int arrayLength = GetArraySize(m_length);
            for (int i = 0; i < arrayLength; i++)
            {
                m_array[i] = ~m_array[i];
            }
            m_version++;
        }

        public bool Any()
        {
            int lastElement = GetArraySize(m_length) - 1;

            if (lastElement < 0)
                return false;

            for (int i = 0; i < lastElement; i++)
            {
                if (m_array[i] != 0)
                    return true;
            }

            return BitMask.AnyFor(m_array[lastElement], f_getAppendixLength());
        }

        public bool All()
        {
            int lastElement = GetArraySize(m_length) - 1;

            if (lastElement < 0)
                return false;

            for (int i = 0; i < lastElement; i++)
            {
                if (m_array[i] != -1)
                    return false;
            }

            return BitMask.AllFor(m_array[lastElement], f_getAppendixLength());
        }

        public bool IsEmpty()
        {
            int arrayLength = GetArraySize(m_length);

            for (int i = 0; i < arrayLength; i++)
            {
                if (m_array[i] != 0)
                    return false;
            }

            return true;
        }

        public int GetCount()
        {
            int lastElement = GetArraySize(m_length) - 1;

            if (lastElement < 0)
                return 0;

            int count = BitMask.GetCount(m_array[lastElement], f_getAppendixLength());

            for (int i = 0; i < lastElement; i++)
            {
                count += BitMask.GetUnitsCount(m_array[i]);
            }

            return count;
        }

        public BitArrayMask Clone()
        {
            return new BitArrayMask(m_array)
            {
                m_version = m_version,
                m_length = m_length
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public int ToIntBitMask(bool throwIfOutOfRange = true)
        {
            if (throwIfOutOfRange && m_length > BitMask.SIZE)
                throw new InvalidOperationException("Bit array has length more than " + BitMask.SIZE.ToString());

            return m_array.Length > 0 ? m_array[0] : 0;
        }

        public BitArray ToBitArray()
        {
            return new BitArray(m_array) { Length = m_length };
        }

        // -- //

        public static BitArrayMask CreateFromBitMask(int bitMask, int length = BitMask.SIZE)
        {
            if (length > BitMask.SIZE || length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), $"Length cannot be negative or more than {BitMask.SIZE}.");

            return new BitArrayMask()
            {
                m_array = new[] { bitMask },
                m_length = length
            };
        }


        internal static int GetArraySize(int bitsCount)
        {
            return bitsCount > 0 ? (bitsCount - 1) / BitMask.SIZE + 1 : 0;
        }

        // -- //

        private int f_getAppendixLength()
        {
            int rem = m_length % BitMask.SIZE;
            return rem == 0 ? BitMask.SIZE : rem;
        }

        // -- //

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<bool> GetEnumerator()
        {
            int ver = m_version;

            for (int i = 0; i < m_length; i++)
            {
                if (ver != m_version)
                    throw Errors.CollectionChanged();

                yield return Get(i);
            }
        }
    }
}
