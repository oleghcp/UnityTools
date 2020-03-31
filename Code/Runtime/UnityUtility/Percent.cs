using UnityUtility.MathExt;
using System;
using UnityEngine;

namespace UnityUtility
{
    [Serializable]
    public struct Percent : IComparable, IComparable<Percent>, IEquatable<Percent>
    {
        public static readonly Percent Zero = new Percent();
        public static readonly Percent Full = new Percent(1f);

        public const float PERCENT_2_RATIO = 0.01f;

        [SerializeField, HideInInspector]
        private float m_value;

        internal static string SerFieldName
        {
            get { return nameof(m_value); }
        }

        private Percent(double ratio)
        {
            m_value = (float)ratio;
        }

        public Percent(float ratio)
        {
            m_value = ratio;
        }

        public Percent(int percent)
        {
            m_value = percent * PERCENT_2_RATIO;
        }

        public void SetValue(float ratio)
        {
            m_value = ratio;
        }

        public void SetValue(int percent)
        {
            m_value = percent * PERCENT_2_RATIO;
        }

        public static Percent Ratio(int left, int right)
        {
            return new Percent((double)left / right);
        }

        public static Percent Parse(string s)
        {
            return new Percent(float.Parse(s));
        }

        public static bool TryParse(string s, out Percent result)
        {
            bool res = float.TryParse(s, out float floatValue);
            result = new Percent(floatValue);
            return res;
        }

        public float ToRatio()
        {
            return m_value;
        }

        public bool Approx(Percent other)
        {
            return Mathf.Approximately(m_value, other.m_value);
        }

        public bool Nearly(Percent other)
        {
            return m_value.Nearly(other.m_value);
        }

        // Regular Stuff //

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Percent))
                throw new ArgumentException("The object must have type of Percent.");

            return CompareTo((Percent)obj);
        }

        public int CompareTo(Percent other)
        {
            return Math.Sign(m_value - other.m_value);
        }

        public bool Equals(Percent other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Percent && this == (Percent)obj;
        }

        public override string ToString()
        {
            return (m_value / PERCENT_2_RATIO).ToString() + "%";
        }

        // Operators //

        public static bool operator ==(Percent a, Percent b)
        {
            return a.m_value == b.m_value;
        }

        public static bool operator !=(Percent a, Percent b)
        {
            return a.m_value != b.m_value;
        }

        public static bool operator >(Percent a, Percent b)
        {
            return a.m_value > b.m_value;
        }

        public static bool operator <(Percent a, Percent b)
        {
            return a.m_value < b.m_value;
        }

        public static bool operator >=(Percent a, Percent b)
        {
            return a.m_value >= b.m_value;
        }

        public static bool operator <=(Percent a, Percent b)
        {
            return a.m_value <= b.m_value;
        }

        public static int operator *(int left, Percent right)
        {
            return (int)((double)left * right.m_value);
        }

        public static Percent operator *(Percent left, int right)
        {
            left.m_value *= right;
            return left;
        }

        public static Percent operator /(Percent left, int right)
        {
            left.m_value /= right;
            return left;
        }

        public static Percent operator *(Percent left, float right)
        {
            left.m_value *= right;
            return left;
        }

        public static Percent operator /(Percent left, float right)
        {
            left.m_value /= right;
            return left;
        }

        public static Percent operator *(Percent left, Percent right)
        {
            left.m_value *= right.m_value;
            return left;
        }

        public static int operator +(int left, Percent right)
        {
            return left + left * right;
        }

        public static int operator -(int left, Percent right)
        {
            return left - left * right;
        }

        public static Percent operator -(Percent a)
        {
            a.m_value = -a.m_value;
            return a;
        }

        public static Percent operator +(Percent a, Percent b)
        {
            a.m_value += b.m_value;
            return a;
        }

        public static Percent operator -(Percent a, Percent b)
        {
            a.m_value -= b.m_value;
            return a;
        }

        public static Percent operator ++(Percent a)
        {
            a.m_value += PERCENT_2_RATIO;
            return a;
        }

        public static Percent operator --(Percent a)
        {
            a.m_value -= PERCENT_2_RATIO;
            return a;
        }
    }
}
