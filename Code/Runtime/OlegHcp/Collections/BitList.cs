using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Collections.Iterators;
using OlegHcp.Tools;

namespace OlegHcp.Collections
{
    //Based on System.Collections.BitArray
#if UNITY
    [Serializable]
#endif
    public class BitList : ICloneable, IReadOnlyList<bool>, IMutable
    {
        private const int MAX_LENGTH = int.MaxValue / BitMask.SIZE;

#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private int[] _array;
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private int _length;
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private bool _mutable;

        private IndexCollection _indices;
        private int _version;

#if UNITY_EDITOR
        internal static string ArrayFieldName => nameof(_array);
        internal static string LengthFieldName => nameof(_length);
        internal static string MutableFieldName => nameof(_mutable);
#endif

        public bool IsReadOnly => !_mutable;
        public int Version => _version;
        int IReadOnlyCollection<bool>.Count => _length;
        internal IReadOnlyList<int> IntBlocks => _array;
        public IndexCollection Indices => _indices ?? (_indices = new IndexCollection(this));

        public int Length
        {
            get => _length;
            set => SetLength(value);
        }

        public bool this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        public BitList(int length, bool defaultValue)
        {
            if (length < 0)
                throw ThrowErrors.NegativeParameter(nameof(length));

            _mutable = true;
            _array = new int[GetArraySize(length)];
            _length = length;
            int num = defaultValue ? (-1) : 0;
            for (int i = 0; i < _array.Length; i++)
            {
                _array[i] = num;
            }
        }

        #region constructor with flag indices
        public BitList(int length, int flagIndex0)
        {
            if (length < 0)
                throw ThrowErrors.NegativeParameter(nameof(length));

            _mutable = true;
            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
        }

        public BitList(int length, int flagIndex0, int flagIndex1)
        {
            if (length < 0)
                throw ThrowErrors.NegativeParameter(nameof(length));

            _mutable = true;
            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
        }

        public BitList(int length, int flagIndex0, int flagIndex1, int flagIndex2)
        {
            if (length < 0)
                throw ThrowErrors.NegativeParameter(nameof(length));

            _mutable = true;
            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
            Set(flagIndex2, true);
        }

        public BitList(int length, int flagIndex0, int flagIndex1, int flagIndex2, int flagIndex3)
        {
            if (length < 0)
                throw ThrowErrors.NegativeParameter(nameof(length));

            _mutable = true;
            _array = new int[GetArraySize(length)];
            _length = length;
            Set(flagIndex0, true);
            Set(flagIndex1, true);
            Set(flagIndex2, true);
            Set(flagIndex3, true);
        }

        public BitList(int length, params int[] indices)
        {
            if (length < 0)
                throw ThrowErrors.NegativeParameter(nameof(length));

            _mutable = true;
            _array = new int[GetArraySize(length)];
            _length = length;
            for (int i = 0; i < indices.Length; i++)
            {
                Set(indices[i], true);
            }
        }
        #endregion

#if UNITY_2021_2_OR_NEWER || !UNITY
        public BitList(bool[] values) : this((Span<bool>)values) { }
#else
        public BitList(bool[] values) : this((ICollection<bool>)values) { }
#endif

        public BitList(ICollection<bool> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            _mutable = true;
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

        public BitList(Span<bool> values)
        {
            _mutable = true;
            _length = values.Length;
            _array = new int[GetArraySize(_length)];

            int i = 0;
            foreach (var item in values)
            {

                if (item)
                    _array[i / BitMask.SIZE] |= 1 << i % BitMask.SIZE;

                i++;
            }
        }

#if UNITY_2021_2_OR_NEWER || !UNITY
        public BitList(int[] intBlocks) : this((Span<int>)intBlocks) { }
#else
        public BitList(int[] intBlocks) : this((ICollection<int>)intBlocks) { }
#endif

        public BitList(ICollection<int> intBlocks)
        {
            if (intBlocks == null)
                throw new ArgumentNullException(nameof(intBlocks));

            if (intBlocks.Count > MAX_LENGTH)
                throw new ArgumentException("Array is too large.", nameof(intBlocks));

            _mutable = true;
            _length = intBlocks.Count * BitMask.SIZE;
            _array = intBlocks.ToArray();
        }


        public BitList(Span<int> intBlocks)
        {
            if (intBlocks.Length > MAX_LENGTH)
                throw new ArgumentException("Array is too large.", nameof(intBlocks));

            _mutable = true;
            _length = intBlocks.Length * BitMask.SIZE;
            _array = intBlocks.ToArray();
        }

        public BitList(BitList bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            _mutable = true;
            int arrayLength = GetArraySize(bits._length);
            _array = new int[arrayLength];
            _length = bits._length;
            Array.Copy(bits._array, _array, arrayLength);
            _version = bits._version;
        }

        public BitList(BitArray bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            _mutable = true;
            int arrayLength = GetArraySize(bits.Length);
            _array = new int[arrayLength];
            _length = bits.Length;
            for (int i = 0; i < _length; i++)
            {
                Set(i, bits[i]);
            }
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
                throw ThrowErrors.ReadOnlyBitList();

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
                throw ThrowErrors.ReadOnlyBitList();

            int num = value ? (-1) : 0;
            int arrayLength = GetArraySize(_length);

            for (int i = 0; i < arrayLength; i++)
            {
                _array[i] = num;
            }

            _version++;
        }

        public void Except(BitList other)
        {
            if (!_mutable)
                throw ThrowErrors.ReadOnlyBitList();

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw ThrowErrors.DifferentArrayLengths();

            int arrayLength = GetArraySize(_length);
            for (int i = 0; i < arrayLength; i++)
            {
                BitMask.Except(ref _array[i], other._array[i]);
            }
            _version++;
        }

        public void And(BitList other)
        {
            if (!_mutable)
                throw ThrowErrors.ReadOnlyBitList();

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw ThrowErrors.DifferentArrayLengths();

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
                throw ThrowErrors.ReadOnlyBitList();

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw ThrowErrors.DifferentArrayLengths();

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
                throw ThrowErrors.ReadOnlyBitList();

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw ThrowErrors.DifferentArrayLengths();

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
                throw ThrowErrors.ReadOnlyBitList();

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

        public bool Intersects(BitList other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (_length != other._length)
                throw ThrowErrors.DifferentArrayLengths();

            int lastElement = GetArraySize(_length) - 1;

            if (lastElement < 0)
                return false;

            for (int i = 0; i < lastElement; i++)
            {
                if ((_array[i] & other._array[i]) == 0)
                    return false;
            }

            return BitMask.Intersect(_array[lastElement], other._array[lastElement], GetAppendixLength());
        }

        public bool Coincides(BitList other)
        {
            if (_length != other._length)
                throw ThrowErrors.DifferentArrayLengths();

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

        public int GetFlagsCount()
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

        #region GetEnumerator
        public Enumerator_<bool> GetEnumerator()
        {
            return new Enumerator_<bool>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator_<bool>(this);
        }

        IEnumerator<bool> IEnumerable<bool>.GetEnumerator()
        {
            return new Enumerator_<bool>(this);
        }
        #endregion

        public BitList GetCopy(bool mutable)
        {
            return new BitList(_array)
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

        public static BitList CreateFromBitMask(IntMask bitMask, int length = BitMask.SIZE)
        {
            return CreateFromBitMask((int)bitMask, length);
        }

        public static BitList CreateFromBitMask(int bitMask, int length = BitMask.SIZE)
        {
            if ((uint)length > BitMask.SIZE)
                throw new ArgumentOutOfRangeException(nameof(length), $"Length cannot be negative or more than {BitMask.SIZE}.");

            Span<int> intBlocks = stackalloc[] { bitMask };
            return new BitList(intBlocks) { _length = length };
        }

        internal static int GetArraySize(int bitsCount)
        {
            return bitsCount > 0 ? (bitsCount - 1) / BitMask.SIZE + 1 : 0;
        }

        private void SetLength(int value)
        {
            if (!_mutable)
                throw ThrowErrors.ReadOnlyBitList();

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

        #region IndexCollection
        public class IndexCollection : IEnumerable<int>
        {
            private BitList _bitList;

            public IndexCollection(BitList bitList)
            {
                _bitList = bitList;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_bitList);
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                return new Enumerator(_bitList);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_bitList);
            }

            public struct Enumerator : IEnumerator<int>
            {
                private readonly BitList _collection;
                private readonly int _version;

                private int _index;
                private int _count;
                private int _current;

                public int Current => _current;

                object IEnumerator.Current
                {
                    get
                    {
                        if (_index == 0 || _index == _count + 1)
                            throw new InvalidOperationException();
                        return _current;
                    }
                }

                public Enumerator(BitList collection)
                {
                    _collection = collection;
                    _index = 0;
                    _count = _collection.Length;
                    _current = default;
                    _version = collection._version;
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (Changed())
                        throw ThrowErrors.CollectionChanged();

                    label:
                    if ((uint)_index < (uint)_count)
                    {
                        if (!_collection[_index])
                        {
                            _index++;
                            goto label;
                        }

                        _current = _index++;
                        return true;
                    }

                    _index = _count + 1;
                    _current = default;
                    return false;
                }

                void IEnumerator.Reset()
                {
                    if (Changed())
                        throw ThrowErrors.CollectionChanged();

                    _index = 0;
                    _current = default;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private bool Changed()
                {
                    return _collection._version != _version;
                }
            }
        }
        #endregion
    }
}
