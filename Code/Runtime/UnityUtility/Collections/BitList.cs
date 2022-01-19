using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtilityTools;

namespace UnityUtility.Collections
{
    //Based on System.Collections.BitArray
    [Serializable]
    public sealed class BitList : ICloneable, IReadOnlyList<bool>
    {
        private const int MAX_LENGTH = int.MaxValue / BitMask.SIZE;

        [SerializeField, HideInInspector]
        private int[] _array;
        [SerializeField, HideInInspector]
        private int _length;
        [SerializeField, HideInInspector]
        private bool _mutable;

        private int _version;

#if UNITY_EDITOR
        internal static string ArrayFieldName => nameof(_array);
        internal static string LengthFieldName => nameof(_length);
#endif

        public bool IsReadOnly => !_mutable;

        public int Count
        {
            get => _length;
            set => SetLength(value);
        }

        public bool this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Get(index);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Set(index, value);
        }

        public int Version => _version;

        internal IReadOnlyList<int> IntBlocks => _array;

        public BitList(int length, bool defaultValue)
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
            _mutable = true;
        }

        #region constructor with flag indices
        public BitList(int length, int flagIndex0)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            _mutable = true;
        }

        public BitList(int length, int flagIndex0, int flagIndex1)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
            _mutable = true;
        }

        public BitList(int length, int flagIndex0, int flagIndex1, int flagIndex2)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
            Set(flagIndex2, true);
            _mutable = true;
        }

        public BitList(int length, int flagIndex0, int flagIndex1, int flagIndex2, int flagIndex3)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
            Set(flagIndex2, true);
            Set(flagIndex3, true);
            _mutable = true;
        }

        public BitList(int length, params int[] indices)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _array = new int[GetArraySize(length)];
            _length = length;
            for (int i = 0; i < indices.Length; i++)
            {
                Set(indices[i], true);
            }
            _mutable = true;
        }
        #endregion

        public BitList(IEnumerable<bool> values)
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
            _mutable = true;
        }

        public BitList(ICollection<bool> values)
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
            _mutable = true;
        }

        public BitList(ICollection<int> intBlocks)
        {
            if (intBlocks == null)
                throw new ArgumentNullException(nameof(intBlocks));

            if (intBlocks.Count > MAX_LENGTH)
                throw new ArgumentException("Array is too large.", nameof(intBlocks));

            _length = intBlocks.Count * BitMask.SIZE;
            _array = intBlocks.ToArray();
            _mutable = true;
        }

        public BitList(Span<int> intBlocks)
        {
            if (intBlocks == null)
                throw new ArgumentNullException(nameof(intBlocks));

            if (intBlocks.Length > MAX_LENGTH)
                throw new ArgumentException("Array is too large.", nameof(intBlocks));

            _length = intBlocks.Length * BitMask.SIZE;
            _array = intBlocks.ToArray();
            _mutable = true;
        }

        public BitList(BitList bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            int arrayLength = GetArraySize(bits._length);
            _array = new int[arrayLength];
            _length = bits._length;
            Array.Copy(bits._array, _array, arrayLength);
            _version = bits._version;
            _mutable = true;
        }

        public bool Get(int index)
        {
            if ((uint)index >= (uint)_length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return (_array[index / BitMask.SIZE] & 1 << index % BitMask.SIZE) != 0;
        }

        public void Set(int index, bool value)
        {
            if (!_mutable)
                throw Errors.ReadOnlyBitList();

            if ((uint)index >= (uint)_length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (value)
                _array[index / BitMask.SIZE] |= 1 << index % BitMask.SIZE;
            else
                _array[index / BitMask.SIZE] &= ~(1 << index % BitMask.SIZE);
            _version++;
            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Switch(int index)
        {
            Set(index, !Get(index));
        }

        public void SetAll(bool value)
        {
            if (!_mutable)
                throw Errors.ReadOnlyBitList();

            int num = value ? (-1) : 0;
            int arrayLength = GetArraySize(_length);

            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] = num;
            }

            _version++;
        }

        public void And(BitList other)
        {
            if (!_mutable)
                throw Errors.ReadOnlyBitList();

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw Errors.DifferentArrayLengths();

            int arrayLength = GetArraySize(_length);
            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] &= other._array[i];
            }
            _version++;
        }

        public void Or(BitList other)
        {
            if (!_mutable)
                throw Errors.ReadOnlyBitList();

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw Errors.DifferentArrayLengths();

            int arrayLength = GetArraySize(_length);
            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] |= other._array[i];
            }
            _version++;
        }

        public void Xor(BitList other)
        {
            if (!_mutable)
                throw Errors.ReadOnlyBitList();

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw Errors.DifferentArrayLengths();

            int arrayLength = GetArraySize(_length);
            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] ^= other._array[i];
            }
            _version++;
        }

        public void Not()
        {
            if (!_mutable)
                throw Errors.ReadOnlyBitList();

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

        public bool Overlaps(BitList other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw Errors.DifferentArrayLengths();

            int lastElement = GetArraySize(_length) - 1;

            if (lastElement < 0)
                return false;

            for (int i = 0; i < lastElement; i++)
            {
                if ((_array[i] & other._array[i]) == 0)
                    return false;
            }

            return BitMask.Overlaps(_array[lastElement], other._array[lastElement], GetAppendixLength());
        }

        public void IntersectWith(BitList other)
        {
            if (!_mutable)
                throw Errors.ReadOnlyBitList();

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw Errors.DifferentArrayLengths();

            int lastElement = GetArraySize(_length) - 1;

            if (lastElement < 0)
                return;

            for (int i = 0; i < lastElement; i++)
            {
                _array[i] &= other._array[i];
            }

            _array[lastElement] = BitMask.GetIntersection(_array[lastElement], other._array[lastElement], GetAppendixLength());

            _version++;
        }

        public bool Coincides(BitList other)
        {
            if (_length != other._length)
                throw Errors.DifferentArrayLengths();

            int lastElement = GetArraySize(_length) - 1;

            if (lastElement < 0)
                return true;

            for (int i = 0; i < lastElement; i++)
            {
                if (_array[i] != other._array[i])
                    return false;
            }

            return BitMask.Equals(_array[lastElement], other._array[lastElement], GetAppendixLength());
        }

        public bool IsEmpty()
        {
            int lastElement = GetArraySize(_length) - 1;

            if (lastElement < 0)
                return true;

            for (int i = 0; i < lastElement; i++)
            {
                if (_array[i] != 0)
                    return false;
            }

            return BitMask.EmptyFor(_array[lastElement], GetAppendixLength());
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

        public BitList GetCopy(bool mutable)
        {
            return new BitList(_array as ICollection<int>)
            {
                _version = _version,
                _length = _length,
                _mutable = mutable,
            };
        }

        object ICloneable.Clone()
        {
            return GetCopy(_mutable);
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

        public static BitList CreateFromBitMask(int bitMask, int length = BitMask.SIZE)
        {
            if ((uint)length > BitMask.SIZE)
                throw new ArgumentOutOfRangeException(nameof(length), $"Length cannot be negative or more than {BitMask.SIZE}.");

            return new BitList(stackalloc[] { bitMask })
            {
                _length = length
            };
        }

        internal static int GetArraySize(int bitsCount)
        {
            return bitsCount > 0 ? (bitsCount - 1) / BitMask.SIZE + 1 : 0;
        }

        private void SetLength(int value)
        {
            if (!_mutable)
                throw Errors.ReadOnlyBitList();

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
