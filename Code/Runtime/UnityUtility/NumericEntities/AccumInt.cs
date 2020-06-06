using System;
using Tools;
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
                throw new ArgumentOutOfRangeException(nameof(spent), "spent value cannot be greater than got value.");

            m_got = got;
            m_spent = spent;
        }

        public void Add(int addValue)
        {
            if (addValue < 0)
                throw Errors.NegativeParameter(nameof(addValue));

            checked { m_got += addValue; }
        }

        public bool Spend(int spendValue)
        {
            if (spendValue < 0)
                throw Errors.NegativeParameter(nameof(spendValue));

            if (spendValue <= Value)
            {
                checked { m_spent += spendValue; }
                return true;
            }

            return false;
        }

        public void Merge(AccumInt other)
        {
            m_got = Math.Max(m_got, other.m_got);
            m_spent = Math.Max(m_spent, other.m_spent);
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is AccumInt && this == (AccumInt)obj;
        }

        public bool Equals(AccumInt other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_got.GetHashCode(), m_spent.GetHashCode());
        }

        public static implicit operator int(AccumInt entity)
        {
            return entity.Value;
        }

        // -- //

        public static bool operator ==(AccumInt a, AccumInt b)
        {
            return a.m_got == b.m_got && a.m_spent == b.m_spent;
        }

        public static bool operator !=(AccumInt a, AccumInt b)
        {
            return !(a == b);
        }

        public static AccumInt operator +(AccumInt value1, int value2)
        {
            if (value2 < 0)
                throw Errors.NegativeParameter(nameof(value2));

            checked { value1.m_got += value2; }
            return value1;
        }

        public static AccumInt operator -(AccumInt value1, int value2)
        {
            if (value2 < 0)
                throw Errors.NegativeParameter(nameof(value2));

            checked { value1.m_spent += value2; }
            return value1;
        }
    }
}
