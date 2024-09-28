using System;
using System.Runtime.CompilerServices;

namespace OlegHcp
{
#if UNITY
    [Serializable]
#endif
    public struct BoolInt : IEquatable<BoolInt>
    {
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private int _value;
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private bool _throwIfNegative;

        public bool ThrowIfNegative => _throwIfNegative;
        public int Count => _value;

        public BoolInt(int defaultValue)
        {
            _value = defaultValue;
            _throwIfNegative = false;
        }

        public BoolInt(bool throwIfNegative)
        {
            _value = 0;
            _throwIfNegative = throwIfNegative;
        }

        public BoolInt(int defaultValue, bool throwIfNegative)
        {
            _value = defaultValue;
            _throwIfNegative = throwIfNegative;
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

        public bool ToBool()
        {
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Increment()
        {
            checked { _value++; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Decrement()
        {
            if (_throwIfNegative && _value == 0)
                throw new InvalidOperationException("Does not support negative values.");

            checked { _value--; }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value, _throwIfNegative);
        }

        public override string ToString()
        {
            return $"{_value > 0} ({_value})";
        }

        public override bool Equals(object obj)
        {
            return obj is BoolInt boolInt && Equals(boolInt);
        }

        public bool Equals(BoolInt other)
        {
            return other._value == _value && other._throwIfNegative == _throwIfNegative;
        }

        public static implicit operator bool(BoolInt value)
        {
            return value._value > 0;
        }

        public static explicit operator int(BoolInt value)
        {
            return value._value;
        }

        public static BoolInt operator ++(BoolInt value)
        {
            value.Increment();
            return value;
        }

        public static BoolInt operator --(BoolInt value)
        {
            value.Decrement();
            return value;
        }
    }
}
