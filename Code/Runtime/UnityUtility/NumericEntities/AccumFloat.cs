using System;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct AccumFloat : IAccumEntity<float>, IEquatable<AccumFloat>, IMergeable<AccumFloat>
    {
        [SerializeField, HideInInspector]
        private float m_got;
        [SerializeField, HideInInspector]
        private float m_spent;

        public float Value
        {
            get { return m_got - m_spent; }
        }

        public bool IsEmpty
        {
            get { return m_got.Nearly(m_spent); }
        }

        public float Got
        {
            get { return m_got; }
        }

        public float Spent
        {
            get { return m_spent; }
        }

        public AccumFloat(float got, float spent)
        {
            if (spent > got)
                throw new ArgumentOutOfRangeException(nameof(spent), "spent value cannot be more than got value.");

            m_got = got;
            m_spent = spent;
        }

        public void Add(float addValue)
        {
            if (addValue < 0f)
                throw new ArgumentOutOfRangeException(nameof(addValue), "value cannot be less than zero.");

            m_got += addValue;
        }

        public bool Spend(float spendValue)
        {
            if (spendValue < 0f)
                throw new ArgumentOutOfRangeException(nameof(spendValue), "value cannot be less than zero.");

            if (spendValue <= Value)
            {
                m_spent += spendValue;
                return true;
            }

            return false;
        }

        public void Merge(AccumFloat other)
        {
            m_got = Math.Max(m_got, other.m_got);
            m_spent = Math.Max(m_spent, other.m_spent);
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is AccumFloat && this == (AccumFloat)obj;
        }

        public bool Equals(AccumFloat other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_got.GetHashCode(), m_spent.GetHashCode());
        }

        public static implicit operator float(AccumFloat entity)
        {
            return entity.Value;
        }

        // -- //

        public static bool operator ==(AccumFloat a, AccumFloat b)
        {
            return a.m_got == b.m_got && a.m_spent == b.m_spent;
        }

        public static bool operator !=(AccumFloat a, AccumFloat b)
        {
            return !(a == b);
        }

        public static AccumFloat operator +(AccumFloat value1, float value2)
        {
            if (value2 < 0f)
                throw new ArgumentOutOfRangeException(nameof(value2), "value cannot be less than zero.");

            value1.m_got += value2;
            return value1;
        }

        public static AccumFloat operator -(AccumFloat value1, float value2)
        {
            if (value2 < 0f)
                throw new ArgumentOutOfRangeException(nameof(value2), "value cannot be less than zero.");

            value1.m_spent += value2;
            return value1;
        }
    }
}
