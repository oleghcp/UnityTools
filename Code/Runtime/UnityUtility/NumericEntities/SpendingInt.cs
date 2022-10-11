using System;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct SpendingInt : ISpendingEntity<int>, IEquatable<SpendingInt>
    {
        [SerializeField, HideInInspector]
        private int _capacity;
        [SerializeField, HideInInspector]
        private int _curValue;

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value < 0)
                    throw Errors.NegativeParameter(nameof(Capacity));

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

        public SpendingInt(int capacity)
        {
            if (capacity < 0)
                throw Errors.NegativeParameter(nameof(capacity));

            _curValue = _capacity = capacity;
        }

        public void Spend(int delta)
        {
            if (delta < 0)
                throw Errors.NegativeParameter(nameof(delta));

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
                throw Errors.NegativeParameter(nameof(delta));

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
            return Helper.GetHashCode(_capacity.GetHashCode(), _curValue.GetHashCode());
        }

        public static implicit operator int(SpendingInt entity)
        {
            return entity.CurValue;
        }
    }
}
