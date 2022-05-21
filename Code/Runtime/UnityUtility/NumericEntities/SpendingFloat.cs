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

        public float Capacity => _capacity;
        public float CurValue => _curValue.CutBefore(0f);
        public float Shortage => (_capacity - _curValue).CutAfter(_capacity);
        public float ReducingExcess => _curValue.CutAfter(0f).Abs();
        public float Ratio => CurValue / _capacity;
        public bool IsFull => _curValue == _capacity;
        public bool IsEmpty => _curValue <= 0f;

        public SpendingFloat(float capacity)
        {
            if (capacity < 0f)
                throw Errors.NegativeParameter(nameof(capacity));

            _curValue = _capacity = capacity;
        }

        public void Spend(float value)
        {
            if (value < 0f)
                throw Errors.NegativeParameter(nameof(value));

            _curValue -= value;
        }

        public void RemoveExcess()
        {
            if (_curValue < 0f)
                _curValue = 0f;
        }

        public void Restore(float value)
        {
            if (value < 0f)
                throw Errors.NegativeParameter(nameof(value));

            _curValue = (_curValue + value).CutAfter(_capacity);
        }

        public void RestoreFull()
        {
            _curValue = _capacity;
        }

        public void Resize(float value, ResizeType resizeType = ResizeType.NewValue)
        {
            switch (resizeType)
            {
                case ResizeType.NewValue:
                    if (value < 0f)
                        throw Errors.NegativeParameter(nameof(value));
                    _capacity = value;
                    _curValue = _curValue.CutAfter(_capacity);
                    break;

                case ResizeType.Delta:
                    float newValue = _capacity += value;
                    _capacity = newValue.CutBefore(0f);
                    _curValue = _curValue.CutAfter(_capacity);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
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
