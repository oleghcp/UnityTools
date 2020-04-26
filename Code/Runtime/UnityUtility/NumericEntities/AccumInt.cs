using System;
using UnityEngine;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct AccumInt : IAccumEntity<int>, IEquatable<AccumInt>
    {
        [SerializeField, HideInInspector]
        private int m_got;
        [SerializeField, HideInInspector]
        private int m_spent;

        public int Value
        {
            get { return m_got - m_spent; }
        }

        public bool IsEmpty
        {
            get { return m_got == m_spent; }
        }

        public int Got
        {
            get { return m_got; }
        }

        public int Spent
        {
            get { return m_spent; }
        }

        public AccumInt(int got, int spent)
        {
            if (spent > got)
                throw new ArgumentOutOfRangeException(nameof(spent), "spent value cannot be more than got value.");

            m_got = got;
            m_spent = spent;
        }

        public void Add(int addValue)
        {
            if (addValue < 0)
                throw new ArgumentOutOfRangeException(nameof(addValue), "value cannot be less than zero.");

            m_got += addValue;
        }

        public bool Spend(int spendValue)
        {
            if (spendValue < 0)
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
            return obj is AccumInt && Equals((AccumInt)obj);
        }

        public bool Equals(AccumInt other)
        {
            return m_got == other.m_got && m_spent == other.m_spent;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_got.GetHashCode(), m_spent.GetHashCode());
        }

        public static implicit operator int(AccumInt entity)
        {
            return entity.Value;
        }
    }
}
