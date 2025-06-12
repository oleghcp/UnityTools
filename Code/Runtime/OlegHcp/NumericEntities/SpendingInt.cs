using System;
using OlegHcp.Mathematics;
using OlegHcp.Tools;

namespace OlegHcp.NumericEntities
{
#if UNITY
    [Serializable]
#endif
    public struct SpendingInt : ISpendingEntity<int>, IEquatable<SpendingInt>
    {
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private int _capacity;
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private int _curValue;

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value < 0)
                    throw ThrowErrors.NegativeParameter(nameof(Capacity));

                _capacity = value;

                if (_curValue > _capacity)
                    _curValue = _capacity;
            }
        }

        public int CurValue
        {
            get => _curValue.ClampMin(0);
            set => _curValue = value.ClampMax(_capacity);
        }

        public int Shortage => (_capacity - _curValue).ClampMax(_capacity);
        public int ReducingExcess => _curValue.ClampMax(0).Abs();
        public float Ratio => NumericHelper.GetRatio(CurValue, _capacity);
        public bool IsEmpty => _curValue <= 0;
        public bool IsFull => _curValue == _capacity;

#if UNITY_EDITOR
        internal static string CapacityFieldName => nameof(_capacity);
        internal static string ValueFieldName => nameof(_curValue);
#endif

        public SpendingInt(int capacity)
        {
            if (capacity < 0)
                throw ThrowErrors.NegativeParameter(nameof(capacity));

            _curValue = _capacity = capacity;
        }

        public SpendingInt(int capacity, int curValue)
        {
            if (capacity < 0)
                throw ThrowErrors.NegativeParameter(nameof(capacity));

            _capacity = capacity;
            _curValue = curValue.ClampMax(capacity);
        }

        public void Spend(int delta)
        {
            if (delta < 0)
                throw ThrowErrors.NegativeParameter(nameof(delta));

            _curValue -= delta;
        }

        public void RemoveExcess()
        {
            if (_curValue < 0)
                _curValue = 0;
        }

        public void Restore(int delta)
        {
            if (delta < 0)
                throw ThrowErrors.NegativeParameter(nameof(delta));

            _curValue = (_curValue + delta).ClampMax(_capacity);
        }

        public void RestoreFull()
        {
            _curValue = _capacity;
        }

        // -- //

        public override string ToString()
        {
            return $"{CurValue}/{Capacity}";
        }

        public override bool Equals(object obj)
        {
            return obj is SpendingInt spendingInt && Equals(spendingInt);
        }

        public bool Equals(SpendingInt other)
        {
            return _curValue == other._curValue && _capacity == other._capacity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_capacity, _curValue);
        }

        public static implicit operator int(SpendingInt entity)
        {
            return entity.CurValue;
        }
    }
}
