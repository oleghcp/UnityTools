﻿using System;
using UnityEngine;
using UnityUtility.Mathematics;

namespace UnityUtility
{
    [Serializable]
    public struct BoolInt : IEquatable<BoolInt>, IComparable<BoolInt>
    {
        [SerializeField, HideInInspector]
        private int _value;

        public BoolInt(int defaultValue)
        {
            _value = defaultValue;
        }

        public void AddValue(bool value)
        {
            if (value)
                Increment();
            else
                Decrement();
        }

        public void AddTrue()
        {
            Increment();
        }

        public void AddFalse()
        {
            Decrement();
        }

        public void Reset()
        {
            _value = 0;
        }

        private void Increment()
        {
            checked { _value++; }
        }

        private void Decrement()
        {
            checked { _value--; }
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return $"{_value.ToBool()} ({_value})";
        }

        public override bool Equals(object obj)
        {
            return obj is BoolInt boolInt && Equals(boolInt);
        }

        public bool Equals(BoolInt other)
        {
            return other._value == _value;
        }

        public int CompareTo(BoolInt other)
        {
            return _value.CompareTo(other._value);
        }

        public static implicit operator bool(BoolInt value)
        {
            return value._value.ToBool();
        }

        public static explicit operator int(BoolInt value)
        {
            return value._value;
        }

        public static bool operator ==(BoolInt a, BoolInt b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BoolInt a, BoolInt b)
        {
            return !a.Equals(b);
        }
    }
}
