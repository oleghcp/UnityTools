using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.BitMasks;

namespace UnityUtility.Collections
{
    //Based on System.Collections.BitArray
    [Serializable]
    public sealed class BitArrayMask : IEnumerable<bool>, ICloneable
    {
        private class BitsIterator : IEnumerable<int>
        {
            public BitArrayMask Owner;

            public IEnumerator<int> GetEnumerator()
            {
                int ver = Owner.m_version;

                for (int i = 0; i < Owner.m_length; i++)
                {
                    if (ver != Owner.m_version)
                        throw new InvalidOperationException("Collection has been changed.");

                    if (Owner.Get(i))
                        yield return i;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [SerializeField, HideInInspector]
        private int[] m_array;
        [SerializeField, HideInInspector]
        private int m_length;

        private int m_version;
        [NonSerialized]
        private BitsIterator m_bits;

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

        public IEnumerable<int> Bits
        {
            get { return m_bits ?? (m_bits = new BitsIterator { Owner = this }); }
        }

        public int Version
        {
            get { return m_version; }
        }

        private BitArrayMask() { }

        public BitArrayMask(int length, bool defaultValue = false)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");

            m_array = new int[GetArrayLength(length, 32)];
            m_length = length;
            int num = defaultValue ? (-1) : 0;
            for (int i = 0; i < m_array.Length; i++)
            {
                m_array[i] = num;
            }
        }

        #region constructor with flag indices
        public BitArrayMask(int length, int flag0)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");

            m_array = new int[GetArrayLength(length, 32)];
            m_length = length;
            Set(flag0, true);
        }

        public BitArrayMask(int length, int flag0, int flag1)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");

            m_array = new int[GetArrayLength(length, 32)];
            m_length = length;
            Set(flag0, true);
            Set(flag1, true);
        }

        public BitArrayMask(int length, int flag0, int flag1, int flag2)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");

            m_array = new int[GetArrayLength(length, 32)];
            m_length = length;
            Set(flag0, true);
            Set(flag1, true);
            Set(flag2, true);
        }

        public BitArrayMask(int length, int flag0, int flag1, int flag2, int flag3)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");

            m_array = new int[GetArrayLength(length, 32)];
            m_length = length;
            Set(flag0, true);
            Set(flag1, true);
            Set(flag2, true);
            Set(flag3, true);
        }

        public BitArrayMask(int length, params int[] indices)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");

            m_array = new int[GetArrayLength(length, 32)];
            m_length = length;
            for (int i = 0; i < indices.Length; i++)
            {
                Set(indices[i], true);
            }
        }
        #endregion

        public BitArrayMask(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length > 268435455)
                throw new ArgumentException("Array is too large.", nameof(bytes));

            m_array = new int[GetArrayLength(bytes.Length, 4)];
            m_length = bytes.Length * 8;
            int num = 0;
            int i;
            for (i = 0; bytes.Length - i >= 4; i += 4)
            {
                m_array[num++] = ((bytes[i] & 0xFF) | (bytes[i + 1] & 0xFF) << 8 | (bytes[i + 2] & 0xFF) << 16 | (bytes[i + 3] & 0xFF) << 24);
            }
            switch (bytes.Length - i)
            {
                case 3:
                    m_array[num] = (bytes[i + 2] & 0xFF) << 16;
                    goto case 2;
                case 2:
                    m_array[num] |= (bytes[i + 1] & 0xFF) << 8;
                    goto case 1;
                case 1:
                    m_array[num] |= (bytes[i] & 0xFF);
                    break;
            }
        }

        public BitArrayMask(bool[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            m_array = new int[GetArrayLength(values.Length, 32)];
            m_length = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i])
                {
                    m_array[i / 32] |= 1 << i % 32;
                }
            }
        }

        public BitArrayMask(int[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length > 67108863)
                throw new ArgumentException("Array is too large.", nameof(values));

            m_array = new int[values.Length];
            m_length = values.Length * 32;
            Array.Copy(values, m_array, values.Length);
        }

        public BitArrayMask(BitArrayMask bits)
        {
            if (bits == null)
                throw new ArgumentNullException("bits");

            int arrayLength = GetArrayLength(bits.m_length, 32);
            m_array = new int[arrayLength];
            m_length = bits.m_length;
            Array.Copy(bits.m_array, m_array, arrayLength);
            m_version = bits.m_version;
        }

        public BitArrayMask(BitArray bits)
        {
            if (bits == null)
                throw new ArgumentNullException("bits");

            var field = typeof(BitArray).GetField("m_array", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            int[] outerArray = field.GetValue(bits) as int[];

            int arrayLength = GetArrayLength(bits.Length, 32);
            m_array = new int[arrayLength];
            m_length = bits.Length;
            Array.Copy(outerArray, m_array, arrayLength);
        }

        public void SetLength(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");

            int arrayLength = GetArrayLength(value, 32);
            if (arrayLength > m_array.Length || arrayLength + 256 < m_array.Length)
            {
                int[] array = new int[arrayLength];
                Array.Copy(m_array, array, (arrayLength > m_array.Length) ? m_array.Length : arrayLength);
                m_array = array;
            }
            if (value > m_length)
            {
                int num = GetArrayLength(m_length, 32) - 1;
                int num2 = m_length % 32;
                if (num2 > 0)
                {
                    m_array[num] &= (1 << num2) - 1;
                }
                Array.Clear(m_array, num + 1, arrayLength - num - 1);
            }
            m_length = value;
            m_version++;
        }

        public bool Get(int index)
        {
            if (index >= 0 && index < m_length)
                return (m_array[index / 32] & 1 << index % 32) != 0;

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public void Set(int index, bool value)
        {
            if (index >= 0 && index < m_length)
            {
                if (value)
                    m_array[index / 32] |= 1 << index % 32;
                else
                    m_array[index / 32] &= ~(1 << index % 32);
                m_version++;
                return;
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public void SetAll(bool value)
        {
            int num = value ? (-1) : 0;
            int arrayLength = GetArrayLength(m_length, 32);
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
                throw new ArgumentException("Array lengths are not equal.");

            int arrayLength = GetArrayLength(m_length, 32);
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
                throw new ArgumentException("Array lengths are not equal.");

            int arrayLength = GetArrayLength(m_length, 32);
            for (int i = 0; i < arrayLength; i++)
            {
                m_array[i] |= value.m_array[i];
            }
            m_version++;
        }

        public void Xor(BitArrayMask value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (m_length != value.m_length)
                throw new ArgumentException("Array lengths are not equal.");

            int arrayLength = GetArrayLength(m_length, 32);
            for (int i = 0; i < arrayLength; i++)
            {
                m_array[i] ^= value.m_array[i];
            }
            m_version++;
        }

        public void Not()
        {
            int arrayLength = GetArrayLength(m_length, 32);
            for (int i = 0; i < arrayLength; i++)
            {
                m_array[i] = ~m_array[i];
            }
            m_version++;
        }

        public bool Any()
        {
            int lastElement = GetArrayLength(m_length, 32) - 1;

            if (lastElement < 0)
                return false;

            for (int i = 0; i < lastElement; i++)
            {
                if (m_array[i] != 0)
                    return true;
            }

            return m_array[lastElement].AnyFor(f_getAppendixLength());
        }

        public bool All()
        {
            int lastElement = GetArrayLength(m_length, 32) - 1;

            if (lastElement < 0)
                return false;

            for (int i = 0; i < lastElement; i++)
            {
                if (m_array[i] != -1)
                    return false;
            }

            return m_array[lastElement].AllFor(f_getAppendixLength());
        }

        public bool IsEmpty()
        {
            int arrayLength = GetArrayLength(m_length, 32);

            for (int i = 0; i < arrayLength; i++)
            {
                if (m_array[i] != 0)
                    return false;
            }

            return true;
        }

        public int GetCount()
        {
            int lastElement = GetArrayLength(m_length, 32) - 1;

            if (lastElement < 0)
                return 0;

            int count = m_array[lastElement].GetCount(f_getAppendixLength());

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
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative or more than 32.");

            return new BitArrayMask()
            {
                m_array = new[] { bitMask },
                m_length = length
            };
        }


        internal static int GetArrayLength(int n, int div)
        {
            return n > 0 ? (n - 1) / div + 1 : 0;
        }

        // -- //

        private int f_getAppendixLength()
        {
            int rem = m_length % 32;
            return rem == 0 ? 32 : rem;
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
                    throw new InvalidOperationException("Collection has been changed.");

                yield return Get(i);
            }
        }
    }
}
