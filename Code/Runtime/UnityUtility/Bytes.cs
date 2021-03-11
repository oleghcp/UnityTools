﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityUtilityTools;

namespace UnityUtility
{
    [Serializable, StructLayout(LayoutKind.Explicit)]
    public struct Bytes : IEquatable<Bytes>, IEnumerable<byte>
    {
        public const int SIZE = sizeof(int);

        [FieldOffset(0), SerializeField, HideInInspector]
        private int _field;
        [FieldOffset(0), NonSerialized]
        private float _floatField;

        public int Size
        {
            get { return SIZE; }
        }

#if UNITY_EDITOR
        internal static string FieldName => nameof(_field);
#endif

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index > SIZE - 1)
                    throw Errors.IndexOutOfRange();

                return GetByteByIndex(_field, index);
            }
            set
            {
                if (index < 0 || index > SIZE - 1)
                    throw Errors.IndexOutOfRange();

                unsafe
                {
                    int val = _field;
                    ((byte*)&val)[index] = value;
                    _field = val;
                }
            }
        }

        public Bytes(int bytes)
        {
            _floatField = 0f;
            _field = bytes;
        }

        public unsafe Bytes(byte b0, byte b1, byte b2, byte b3)
        {
            _floatField = 0f;
            byte* ptr = stackalloc[] { b0, b1, b2, b3 };
            _field = *(int*)ptr;
        }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(_field);
        }

        // -- //        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe byte GetByteByIndex(int field, int index)
        {
            return ((byte*)&field)[index];
        }

        // -- //

        #region regular stuff
        public override int GetHashCode()
        {
            return _field;
        }

        public override bool Equals(object obj)
        {
            return obj is Bytes && this == (Bytes)obj;
        }

        public bool Equals(Bytes other)
        {
            return this == other;
        }

        public override unsafe string ToString()
        {
            int val = _field;
            byte* ptr = (byte*)&val;

            StringBuilder sb = new StringBuilder();
            sb.Append('(').Append(ptr[0]);
            sb.Append('.').Append(ptr[1]);
            sb.Append('.').Append(ptr[2]);
            sb.Append('.').Append(ptr[3]);
            return sb.Append(')').ToString();
        }

        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i < SIZE; i++)
            {
                yield return GetByteByIndex(_field, i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        // Operators //

        public static bool operator ==(Bytes a, Bytes b)
        {
            return a._field == b._field;
        }

        public static bool operator !=(Bytes a, Bytes b)
        {
            return a._field != b._field;
        }

        // -- //

        public static explicit operator float(Bytes bytes) { return bytes._floatField; }

        public static explicit operator int(Bytes bytes) { return bytes._field; }

        public static explicit operator uint(Bytes bytes) { return (uint)bytes._field; }

        public static explicit operator short(Bytes bytes) { return (short)bytes._field; }

        public static explicit operator ushort(Bytes bytes) { return (ushort)bytes._field; }

        public static explicit operator bool(Bytes bytes) { return bytes._field != 0; }

        public static explicit operator LayerMask(Bytes bytes) { return bytes._field; }

        public static unsafe explicit operator Color32(Bytes bytes)
        {
            int val = bytes._field;
            byte* ptr = (byte*)&val;
            return new Color32(ptr[0], ptr[1], ptr[2], ptr[3]);
        }

        // -- //

        public static implicit operator Bytes(float val) { return new Bytes { _floatField = val }; }

        public static implicit operator Bytes(int val) { return new Bytes { _field = val }; }

        public static implicit operator Bytes(uint val) { return new Bytes { _field = (int)val }; }

        public static implicit operator Bytes(short val) { return new Bytes { _field = val }; }

        public static implicit operator Bytes(ushort val) { return new Bytes { _field = val }; }

        public static implicit operator Bytes(bool val) { return new Bytes { _field = val ? 1 : 0 }; }

        public static implicit operator Bytes(LayerMask val) { return new Bytes { _field = val }; }

        public static implicit operator Bytes(Color32 val) { return new Bytes(val.r, val.g, val.b, val.a); }
    }
}
