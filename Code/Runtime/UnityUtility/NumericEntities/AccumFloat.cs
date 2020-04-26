using UnityUtility.MathExt;
using System;
using UnityEngine;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct AccumFloat : IAccumEntity<float>, IEquatable<AccumFloat>
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

        // -- //

        public override bool Equals(object obj)
        {
            return obj is AccumFloat && Equals((AccumFloat)obj);
        }

        public bool Equals(AccumFloat other)
        {
            return m_got == other.m_got && m_spent == other.m_spent;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_got.GetHashCode(), m_spent.GetHashCode());
        }

        public static implicit operator float(AccumFloat entity)
        {
            return entity.Value;
        }
    }
}
