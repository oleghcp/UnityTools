using System;
using OlegHcp.Mathematics;
using OlegHcp.Tools;

namespace OlegHcp.NumericEntities
{
#if UNITY
    [Serializable]
#endif
    public struct SpendingFloat : ISpendingEntity<float>, IEquatable<SpendingFloat>
    {
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private float _capacity;
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private float _curValue;

        public float Capacity
        {
            get => _capacity;
            set
            {
                if (value < 0f)
                    throw ThrowErrors.NegativeParameter(nameof(Capacity));

                _capacity = value;

                if (_curValue > _capacity)
                    _curValue = _capacity;
            }
        }

        public float CurValue
        {
            get => _curValue.ClampMin(0f);
            set => _curValue = value.ClampMax(_capacity);
        }

        public float Shortage => (_capacity - _curValue).ClampMax(_capacity);
        public float ReducingExcess => _curValue.ClampMax(0f).Abs();
        public float Ratio => NumericHelper.GetRatio(CurValue, _capacity);
        public bool IsFull => _curValue == _capacity;
        public bool IsEmpty => _curValue <= 0f;

#if UNITY_EDITOR
        internal static string CapacityFieldName => nameof(_capacity);
        internal static string ValueFieldName => nameof(_curValue);
#endif

        public SpendingFloat(float capacity)
        {
            if (capacity < 0f)
                throw ThrowErrors.NegativeParameter(nameof(capacity));

            _curValue = _capacity = capacity;
        }

        public SpendingFloat(float capacity, float curValue)
        {
            if (capacity < 0f)
                throw ThrowErrors.NegativeParameter(nameof(capacity));

            _capacity = capacity;
            _curValue = curValue.ClampMax(capacity);
        }

        public void Spend(float delta)
        {
            if (delta < 0f)
                throw ThrowErrors.NegativeParameter(nameof(delta));

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
            return obj is SpendingFloat spendingFloat && Equals(spendingFloat);
        }

        public bool Equals(SpendingFloat other)
        {
            return _curValue == other._curValue && _capacity == other._capacity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_capacity, _curValue);
        }

        public static implicit operator float(SpendingFloat entity)
        {
            return entity.CurValue;
        }
    }
}
