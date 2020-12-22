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
        private float m_capacity;
        [SerializeField, HideInInspector]
        private float m_curValue;

        public float Capacity
        {
            get { return m_capacity; }
        }

        public float CurValue
        {
            get { return m_curValue.CutBefore(0f); }
        }

        public float Shortage
        {
            get { return (m_capacity - m_curValue).CutAfter(m_capacity); }
        }

        public float ReducingExcess
        {
            get { return m_curValue.CutAfter(0f).Abs(); }
        }

        public float Ratio
        {
            get { return CurValue / m_capacity; }
        }

        public bool IsFull
        {
            get { return m_curValue == m_capacity; }
        }

        public bool IsEmpty
        {
            get { return m_curValue <= 0f; }
        }

        public SpendingFloat(float capacity)
        {
            if (capacity < 0f)
                throw Errors.NegativeParameter(nameof(capacity));

            m_curValue = m_capacity = capacity;
        }

        public void Spend(float value)
        {
            if (value < 0f)
                throw Errors.NegativeParameter(nameof(value));

            m_curValue -= value;
        }

        public void RemoveExcess()
        {
            if (m_curValue < 0f)
                m_curValue = 0f;
        }

        public void Restore(float value)
        {
            if (value < 0f)
                throw Errors.NegativeParameter(nameof(value));

            m_curValue = (m_curValue + value).CutAfter(m_capacity);
        }

        public void RestoreFull()
        {
            m_curValue = m_capacity;
        }

        public void Resize(float value, ResizeType resizeType = ResizeType.NewValue)
        {
            if (value < 0f)
                throw Errors.NegativeParameter(nameof(value));

            switch (resizeType)
            {
                case ResizeType.NewValue:
                    m_capacity = value;
                    m_curValue = m_curValue.CutAfter(m_capacity);
                    break;

                case ResizeType.Increase:
                    m_capacity += value;
                    break;

                case ResizeType.Decrease:
                    m_capacity -= value.CutAfter(m_capacity);
                    m_curValue = m_curValue.CutAfter(m_capacity);
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
            return obj is SpendingFloat && Equals((SpendingFloat)obj);
        }

        public bool Equals(SpendingFloat other)
        {
            return m_curValue == other.m_curValue && m_capacity == other.m_capacity;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_capacity.GetHashCode(), m_curValue.GetHashCode());
        }

        public static implicit operator float(SpendingFloat entity)
        {
            return entity.CurValue;
        }
    }
}
