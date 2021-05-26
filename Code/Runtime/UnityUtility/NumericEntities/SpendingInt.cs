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
        private int m_capacity;
        [SerializeField, HideInInspector]
        private int m_curValue;

        public int Capacity => m_capacity;
        public int CurValue => m_curValue.CutBefore(0);
        public int Shortage => (m_capacity - m_curValue).CutAfter(m_capacity);
        public int ReducingExcess => m_curValue.CutAfter(0).Abs();
        public bool IsEmpty => m_curValue <= 0;
        public bool IsFull => m_curValue == m_capacity;
        public float Ratio => (float)CurValue / m_capacity;

        public SpendingInt(int capacity)
        {
            if (capacity < 0)
                throw Errors.NegativeParameter(nameof(capacity));

            m_curValue = m_capacity = capacity;
        }

        public void Spend(int value)
        {
            if (value < 0)
                throw Errors.NegativeParameter(nameof(value));

            m_curValue -= value;
        }

        public void RemoveExcess()
        {
            if (m_curValue < 0)
                m_curValue = 0;
        }

        public void Restore(int value)
        {
            if (value < 0)
                throw Errors.NegativeParameter(nameof(value));

            m_curValue = (m_curValue + value).CutAfter(m_capacity);
        }

        public void RestoreFull()
        {
            m_curValue = m_capacity;
        }

        public void Resize(int value, ResizeType resizeType = ResizeType.NewValue)
        {
            switch (resizeType)
            {
                case ResizeType.NewValue:
                    if (value < 0)
                        throw Errors.NegativeParameter(nameof(value));
                    m_capacity = value;
                    m_curValue = m_curValue.CutAfter(m_capacity);
                    break;

                case ResizeType.Delta:
                    int newValue = m_capacity += value;
                    m_capacity = newValue.CutBefore(0);
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
            return obj is SpendingInt spendingInt && Equals(spendingInt);
        }

        public bool Equals(SpendingInt other)
        {
            return m_curValue == other.m_curValue && m_capacity == other.m_capacity;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_capacity.GetHashCode(), m_curValue.GetHashCode());
        }

        public static implicit operator int(SpendingInt entity)
        {
            return entity.CurValue;
        }
    }
}
