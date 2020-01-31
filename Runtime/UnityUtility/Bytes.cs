using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UU.Collections;
using UnityEngine;

namespace UU
{
    [Serializable, StructLayout(LayoutKind.Explicit)]
    public struct Bytes : IEquatable<Bytes>, IEnumerable<byte>
    {
        public const int SIZE = sizeof(int);

        [FieldOffset(0), SerializeField, HideInInspector]
        private int m_field;
        [FieldOffset(0), NonSerialized]
        private float m_floatField;

        public int Size
        {
            get { return SIZE; }
        }

        internal static string SerFieldName
        {
            get { return nameof(m_field); }
        }

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index > SIZE - 1)
                    throw new IndexOutOfRangeException("The index is out of range.");

                return f_get(m_field, index);
            }
            set
            {
                if (index < 0 || index > SIZE - 1)
                    throw new IndexOutOfRangeException("The index is out of range.");

                unsafe
                {
                    int val = m_field;
                    ((byte*)&val)[index] = value;
                    m_field = val;
                }
            }
        }

        public Bytes(int bytes)
        {
            m_floatField = 0f;
            m_field = bytes;
        }

        public unsafe Bytes(byte b0, byte b1, byte b2, byte b3)
        {
            m_floatField = 0f;
            byte* ptr = stackalloc[] { b0, b1, b2, b3 };
            m_field = *(int*)ptr;
        }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(m_field);
        }

        // -- //        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe byte f_get(int field, int index)
        {
            return ((byte*)&field)[index];
        }

        // -- //

        #region regular stuff
        public override int GetHashCode()
        {
            return m_field;
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
            int val = m_field;
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
                yield return f_get(m_field, i);
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
            return a.m_field == b.m_field;
        }

        public static bool operator !=(Bytes a, Bytes b)
        {
            return a.m_field != b.m_field;
        }

        // -- //

        public static explicit operator float(Bytes bytes) { return bytes.m_floatField; }

        public static explicit operator int(Bytes bytes) { return bytes.m_field; }

        public static explicit operator uint(Bytes bytes) { return (uint)bytes.m_field; }

        public static explicit operator short(Bytes bytes) { return (short)bytes.m_field; }

        public static explicit operator ushort(Bytes bytes) { return (ushort)bytes.m_field; }

        public static explicit operator bool(Bytes bytes) { return bytes.m_field != 0; }

        public static explicit operator Percent(Bytes bytes) { return new Percent(bytes.m_floatField); }

        public static explicit operator LayerMask(Bytes bytes) { return bytes.m_field; }

        public static unsafe explicit operator Color32(Bytes bytes)
        {
            int val = bytes.m_field;
            byte* ptr = (byte*)&val;
            return new Color32(ptr[0], ptr[1], ptr[2], ptr[3]);
        }

        // -- //

        public static implicit operator Bytes(float val) { return new Bytes { m_floatField = val }; }

        public static implicit operator Bytes(int val) { return new Bytes { m_field = val }; }

        public static implicit operator Bytes(uint val) { return new Bytes { m_field = (int)val }; }

        public static implicit operator Bytes(short val) { return new Bytes { m_field = val }; }

        public static implicit operator Bytes(ushort val) { return new Bytes { m_field = val }; }

        public static implicit operator Bytes(bool val) { return new Bytes { m_field = val ? 1 : 0 }; }

        public static implicit operator Bytes(Percent val) { return new Bytes { m_floatField = val.ToRatio() }; }

        public static implicit operator Bytes(LayerMask val) { return new Bytes { m_field = val }; }

        public static implicit operator Bytes(Color32 val) { return new Bytes(val.r, val.g, val.b, val.a); }
    }
}
