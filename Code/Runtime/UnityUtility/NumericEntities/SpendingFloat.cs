using System;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct SpendingFloat : ISpendingEntity<float>, IEquatable<SpendingFloat>
    {
        [SerializeField, HideInInspector]
        private float _capacity;
        [SerializeField, HideInInspector]
        private float _curValue;

        public float Capacity
        {
            get => _capacity;
            set
            {
                if (value < 0f)
                    throw Errors.NegativeParameter(nameof(Capacity));

                _capacity = value;

                if (_curValue > _capacity)
                    _curValue = _capacity;
            }
        }

        public float CurValue
        {
            get => _curValue.CutBefore(0f);
            set => _curValue = value.CutAfter(_capacity);
        }

        public float Shortage => (_capacity - _curValue).CutAfter(_capacity);
        public float ReducingExcess => _curValue.CutAfter(0f).Abs();
        public float Ratio => NumericHelper.GetRatio(CurValue, _capacity);
        public bool IsFull => _curValue == _capacity;
        public bool IsEmpty => _curValue <= 0f;

        public SpendingFloat(float capacity)
        {
            if (capacity < 0f)
                throw Errors.NegativeParameter(nameof(capacity));

            _curValue = _capacity = capacity;
        }

        public void Spend(float delta)
        {
            if (delta < 0f)
                throw Errors.NegativeParameter(nameof(delta));

            _curValue -= delta;
        }

        public void RemoveExcess()
        {
            if (_curValue < 0f)
                _curValue = 0f;
        }

        public void Restore(float delta)
        {
            if (delta < 0f)
                throw Errors.NegativeParameter(nameof(delta));

            _curValue = (_curValue + delta).CutAfter(_capacity);
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
            return obj is SpendingFloat spendingFloat && Equals(spendingFloat);
        }

        public bool Equals(SpendingFloat other)
        {
            return _curValue == other._curValue && _capacity == other._capacity;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(_capacity.GetHashCode(), _curValue.GetHashCode());
        }

        public static implicit operator float(SpendingFloat entity)
        {
            return entity.CurValue;
        }
    }
}
