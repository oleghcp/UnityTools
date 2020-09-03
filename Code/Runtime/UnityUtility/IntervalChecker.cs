using System;
using UnityUtilityTools;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtility
{
    [Serializable]
    public struct IntervalChecker
    {
        [SerializeField]
        private float m_interval;
        [SerializeField]
        private float m_currentValue;

        public float Interval
        {
            get { return m_interval; }
            set
            {
                if (value <= 0f)
                    throw Errors.ZeroParameter(nameof(value));

                m_interval = value;
            }
        }

        public float CurrentValue
        {
            get { return m_currentValue; }
            set { m_currentValue = value.CutAfter(m_interval); }
        }

        public float Ratio
        {
            get { return m_currentValue / m_interval; }
            set { m_currentValue = m_interval * value.Saturate(); }
        }

        public float Shortage
        {
            get { return (m_interval - m_currentValue).CutBefore(0f); }
        }

        public IntervalChecker(float interval)
        {
            if (interval <= 0f)
                throw Errors.ZeroParameter(nameof(interval));

            m_interval = interval;
            m_currentValue = 0f;
        }

        public IntervalChecker(float interval, float startValue)
        {
            if (interval <= 0f)
                throw Errors.ZeroParameter(nameof(interval));

            m_interval = interval;
            m_currentValue = startValue.CutAfter(interval);
        }

        public bool HardCheckDelta(float deltaValue)
        {
            if (deltaValue < 0f)
                Errors.NegativeParameter(nameof(deltaValue));

            return HardCheckValue(m_currentValue + deltaValue);
        }

        public bool HardCheckValue(float newValue)
        {
            if (newValue >= m_interval)
            {
                m_currentValue = 0f;
                return true;
            }

            m_currentValue = newValue;
            return false;
        }

        public bool SmoothCheckDelta(float deltaValue)
        {
            if (deltaValue < 0f)
                Errors.NegativeParameter(nameof(deltaValue));

            return SmoothCheckValue(m_currentValue + deltaValue);
        }

        public bool SmoothCheckValue(float newValue)
        {
            if (newValue >= m_interval)
            {
                m_currentValue = newValue - m_interval;
                return true;
            }

            m_currentValue = newValue;
            return false;
        }
    }
}
