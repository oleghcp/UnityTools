using UU.MathExt;
using System;
using UnityEngine;

namespace UU.NumericEntities
{
    [Serializable]
    public struct ForcedInt : ForcedEntity<int>, IEquatable<ForcedInt>
    {
        [SerializeField, HideInInspector]
        private int m_threshold;
        [SerializeField, HideInInspector]
        private int m_expander;

        public int CurValue
        {
            get { return m_expander.Clamp(0, m_threshold); }
        }

        public int Threshold
        {
            get { return m_threshold; }
        }

        public bool LimitReached
        {
            get { return m_expander >= m_threshold; }
        }

        public bool Forced
        {
            get { return m_expander > 0; }
        }

        public float Ratio
        {
            get { return (float)m_expander / m_threshold; }
        }

        public ForcedInt(int threshold)
        {
            if (threshold < 0)
                throw new ArgumentOutOfRangeException(nameof(threshold), "threshold cannot be less than zero.");

            m_threshold = threshold;
            m_expander = 0;
        }

        public void Force(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");

            m_expander += value.Clamp(0, m_threshold - m_expander);
        }

        public void Restore(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");

            m_expander -= value.Clamp(0, m_expander);
        }

        public void RestoreFull()
        {
            m_expander = 0;
        }

        public void Resize(int value, ResizeType resizeType = ResizeType.NewValue)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");

            switch (resizeType)
            {
                case ResizeType.NewValue:
                    m_threshold = value;
                    m_expander = m_expander.Clamp(0, m_threshold);
                    break;

                case ResizeType.Increase:
                    m_threshold += value;
                    break;

                case ResizeType.Decrease:
                    m_threshold -= value.Clamp(0, m_threshold);
                    m_expander = m_expander.Clamp(0, m_threshold);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is ForcedInt && Equals((ForcedInt)obj);
        }

        public bool Equals(ForcedInt other)
        {
            return m_expander == other.m_expander && m_threshold == other.m_threshold;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_expander.GetHashCode(), m_threshold.GetHashCode());
        }
    }
}
