using UU.MathExt;
using System;
using UnityEngine;

namespace UU.NumericEntities
{
    [Serializable]
    public struct ForcedFloat : ForcedEntity<float>, IEquatable<ForcedFloat>
    {
        [SerializeField, HideInInspector]
        private float m_threshold;
        [SerializeField, HideInInspector]
        private float m_expander;

        public float CurValue
        {
            get { return m_expander.Clamp(0f, m_threshold); }
        }

        public float Threshold
        {
            get { return m_threshold; }
        }

        public bool LimitReached
        {
            get { return m_expander >= m_threshold; }
        }

        public bool Forced
        {
            get { return m_expander > float.Epsilon; }
        }

        public float Ratio
        {
            get { return m_expander / m_threshold; }
        }

        public ForcedFloat(float threshold)
        {
            if (threshold < 0f)
                throw new ArgumentOutOfRangeException(nameof(threshold), "threshold cannot be less than zero.");

            m_threshold = threshold;
            m_expander = 0f;
        }

        public void Force(float value)
        {
            if (value < 0f)
                throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");

            m_expander += value.Clamp(0f, m_threshold - m_expander);
        }

        public void Restore(float value)
        {
            if (value < 0f)
                throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");

            m_expander -= value.Clamp(0f, m_expander);
        }

        public void RestoreFull()
        {
            m_expander = 0f;
        }

        public void Resize(float value, ResizeType resizeType = ResizeType.NewValue)
        {
            if (value < 0f)
                throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");

            switch (resizeType)
            {
                case ResizeType.NewValue:
                    m_threshold = value;
                    m_expander = m_expander.Clamp(0f, m_threshold);
                    break;

                case ResizeType.Increase:
                    m_threshold += value;
                    break;

                case ResizeType.Decrease:
                    m_threshold -= value.Clamp(0f, m_threshold);
                    m_expander = m_expander.Clamp(0f, m_threshold);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is ForcedFloat && Equals((ForcedFloat)obj);
        }

        public bool Equals(ForcedFloat other)
        {
            return m_expander == other.m_expander && m_threshold == other.m_threshold;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_expander.GetHashCode(), m_threshold.GetHashCode());
        }
    }
}
