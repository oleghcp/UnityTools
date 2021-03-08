﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private int[] _array;
        [SerializeField, HideInInspector]
        private int _length;

        private int _version;

        internal static string ArrayFieldName
        {
            get { return nameof(_array); }
        }

        internal static string LengthFieldName
        {
            get { return nameof(_length); }
        }

        public bool this[int index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }

        public int Length
        {
            get { return _length; }
            set { SetLength(value); }
        }

        public int Version
        {
            get { return _version; }
        }

        int IReadOnlyCollection<bool>.Count
        {
            get { return _length; }
        }

        [Preserve]
        private BitArrayMask() { }

        public BitArrayMask(int length, bool defaultValue = false)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            int num = defaultValue ? (-1) : 0;
            for (int i = 0; i < _array.Length; i++)
            {
                _array[i] = num;
            }
        }

        #region constructor with flag indices
        public BitArrayMask(int length, int flagIndex0)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
        }

        public BitArrayMask(int length, int flagIndex0, int flagIndex1)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
        }

        public BitArrayMask(int length, int flagIndex0, int flagIndex1, int flagIndex2)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
            Set(flagIndex2, true);
        }

        public BitArrayMask(int length, int flagIndex0, int flagIndex1, int flagIndex2, int flagIndex3)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
            Set(flagIndex2, true);
            Set(flagIndex3, true);
        }

        public BitArrayMask(int length, params int[] indices)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
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

            _array = new int[2];

            int i = 0;
            foreach (var item in values)
            {
                if (i >= _array.Length * BitMask.SIZE)
                    Array.Resize(ref _array, _array.Length * 2);

                if (item)
                    _array[i / BitMask.SIZE] |= 1 << i % BitMask.SIZE;

                i++;
            }

            _length = i;
        }

        public BitArrayMask(ICollection<bool> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            _length = values.Count;
            _array = new int[GetArraySize(_length)];

            int i = 0;
            foreach (var item in values)
            {

                if (item)
                    _array[i / BitMask.SIZE] |= 1 << i % BitMask.SIZE;

                i++;
            }
        }

        private BitArrayMask(ICollection<int> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Count > 67108863)
                throw new ArgumentException("Array is too large.", nameof(values));

            _length = values.Count * BitMask.SIZE;
            _array = values.ToArray();
        }

        public BitArrayMask(BitArrayMask bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            int arrayLength = GetArraySize(bits._length);
            _array = new int[arrayLength];
            _length = bits._length;
            Array.Copy(bits._array, _array, arrayLength);
            _version = bits._version;
        }

        public bool Get(int index)
        {
            if (index >= 0 && index < _length)
                return (_array[index / BitMask.SIZE] & 1 << index % BitMask.SIZE) != 0;

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public void Set(int index, bool value)
        {
            if (index >= 0 && index < _length)
            {
                if (value)
                    _array[index / BitMask.SIZE] |= 1 << index % BitMask.SIZE;
                else
                    _array[index / BitMask.SIZE] &= ~(1 << index % BitMask.SIZE);
                _version++;
                return;
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public void SetAll(bool value)
        {
            int num = value ? (-1) : 0;
            int arrayLength = GetArraySize(_length);
            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] = num;
            }
            _version++;
        }

        public void And(BitArrayMask value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (_length != value._length)
                throw Errors.DifferentArrayLengths();

            int arrayLength = GetArraySize(_length);
            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] &= value._array[i];
            }
            _version++;
        }

        public void Or(BitArrayMask value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (_length != value._length)
                throw Errors.DifferentArrayLengths();

            int arrayLength = GetArraySize(_length);
            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] |= value._array[i];
            }
            _version++;
        }

        public void Xor(BitArrayMask value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (_length != value._length)
                throw Errors.DifferentArrayLengths();

            int arrayLength = GetArraySize(_length);
            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] ^= value._array[i];
            }
            _version++;
        }

        public void Not()
        {
            int arrayLength = GetArraySize(_length);
            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] = ~_array[i];
            }
            _version++;
        }

        public bool Any()
        {
            int lastElement = GetArraySize(_length) - 1;

            if (lastElement < 0)
                return false;

            for (int i = 0; i < lastElement; i++)
            {
                if (_array[i] != 0)
                    return true;
            }

            return BitMask.AnyFor(_array[lastElement], GetAppendixLength());
        }

        public bool All()
        {
            int lastElement = GetArraySize(_length) - 1;

            if (lastElement < 0)
                return false;

            for (int i = 0; i < lastElement; i++)
            {
                if (_array[i] != -1)
                    return false;
            }

            return BitMask.AllFor(_array[lastElement], GetAppendixLength());
        }

        public bool IsEmpty()
        {
            int arrayLength = GetArraySize(_length);

            for (int i = 0; i < arrayLength; i++)
            {
                if (_array[i] != 0)
                    return false;
            }

            return true;
        }

        public int GetCount()
        {
            int lastElement = GetArraySize(_length) - 1;

            if (lastElement < 0)
                return 0;

            int count = BitMask.GetCount(_array[lastElement], GetAppendixLength());

            for (int i = 0; i < lastElement; i++)
            {
                count += BitMask.GetUnitsCount(_array[i]);
            }

            return count;
        }

        public BitArrayMask Clone()
        {
            return new BitArrayMask(_array)
            {
                _version = _version,
                _length = _length
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public int ToIntBitMask(bool throwIfOutOfRange = true)
        {
            if (throwIfOutOfRange && _length > BitMask.SIZE)
                throw new InvalidOperationException("Bit array has length more than " + BitMask.SIZE.ToString());

            return _array.Length > 0 ? _array[0] : 0;
        }

        public BitArray ToBitArray()
        {
            return new BitArray(_array) { Length = _length };
        }

        // -- //

        public static BitArrayMask CreateFromBitMask(int bitMask, int length = BitMask.SIZE)
        {
            if (length > BitMask.SIZE || length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), $"Length cannot be negative or more than {BitMask.SIZE}.");

            return new BitArrayMask()
            {
                _array = new[] { bitMask },
                _length = length
            };
        }

        internal static int GetArraySize(int bitsCount)
        {
            return bitsCount > 0 ? (bitsCount - 1) / BitMask.SIZE + 1 : 0;
        }

        private void SetLength(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");

            int newArraySize = GetArraySize(value);

            if (newArraySize > _array.Length || newArraySize + 256 < _array.Length)
            {
                Array.Resize(ref _array, newArraySize);
            }

            if (value > _length)
            {
                int num = GetArraySize(_length) - 1;
                int num2 = _length % BitMask.SIZE;
                if (num2 > 0)
                {
                    _array[num] &= (1 << num2) - 1;
                }
                Array.Clear(_array, num + 1, newArraySize - num - 1);
            }

            _length = value;
            _version++;
        }

        // -- //

        private int GetAppendixLength()
        {
            int rem = _length % BitMask.SIZE;
            return rem == 0 ? BitMask.SIZE : rem;
        }

        // -- //

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<bool> GetEnumerator()
        {
            int ver = _version;

            for (int i = 0; i < _length; i++)
            {
                if (ver != _version)
                    throw Errors.CollectionChanged();

                yield return Get(i);
            }
        }
    }
}
