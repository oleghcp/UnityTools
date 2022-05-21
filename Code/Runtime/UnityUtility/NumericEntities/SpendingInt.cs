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

        public int Capacity => _capacity;
        public int CurValue => _curValue.CutBefore(0);
        public int Shortage => (_capacity - _curValue).CutAfter(_capacity);
        public int ReducingExcess => _curValue.CutAfter(0).Abs();
        public bool IsEmpty => _curValue <= 0;
        public bool IsFull => _curValue == _capacity;
        public float Ratio => (float)CurValue / _capacity;

        public SpendingInt(int capacity)
        {
            if (capacity < 0)
                throw Errors.NegativeParameter(nameof(capacity));

            _curValue = _capacity = capacity;
        }

        public void Spend(int value)
        {
            if (value < 0)
                throw Errors.NegativeParameter(nameof(value));

            _curValue -= value;
        }

        public void RemoveExcess()
        {
            if (_curValue < 0)
                _curValue = 0;
        }

        public void Restore(int value)
        {
            if (value < 0)
                throw Errors.NegativeParameter(nameof(value));

            _curValue = (_curValue + value).CutAfter(_capacity);
        }

        public void RestoreFull()
        {
            _curValue = _capacity;
        }

        public void Resize(int value, ResizeType resizeType = ResizeType.NewValue)
        {
            switch (resizeType)
            {
                case ResizeType.NewValue:
                    if (value < 0)
                        throw Errors.NegativeParameter(nameof(value));
                    _capacity = value;
                    _curValue = _curValue.CutAfter(_capacity);
                    break;

                case ResizeType.Delta:
                    int newValue = _capacity += value;
                    _capacity = newValue.CutBefore(0);
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
