using UnityUtility.MathExt;
using System;
using UnityEngine;
using Tools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct FilledInt : IFilledEntity<int>, IEquatable<FilledInt>
    {
        [SerializeField, HideInInspector]
        private int m_threshold;
        [SerializeField, HideInInspector]
        private int m_filler;

        public int CurValue
        {
            get { return m_filler.Clamp(0, m_threshold); }
        }

        public int Threshold
        {
            get { return m_threshold; }
        }

        public bool FilledFully
        {
            get { return m_filler >= m_threshold; }
        }

        public bool IsEmpty
        {
            get { return m_filler == 0; }
        }

        public float Ratio
        {
            get { return (float)m_filler / m_threshold; }
        }

        public FilledInt(int threshold)
        {
            if (threshold < 0)
                throw new ArgumentOutOfRangeException(nameof(threshold), "threshold cannot be less than zero.");

            m_threshold = threshold;
            m_filler = 0;
        }

        public void Fill(int addValue)
        {
            if (addValue < 0)
                throw new ArgumentOutOfRangeException(nameof(addValue), "value cannot be less than zero.");

            m_filler += addValue.Clamp(0, m_threshold - m_filler);
        }

        public void FillFully()
        {
            m_filler = m_threshold;
        }

        public void Remove(int removeValue)
        {
            if (removeValue < 0)
                throw new ArgumentOutOfRangeException(nameof(removeValue), "value cannot be less than zero.");

            m_filler -= removeValue.Clamp(0, m_filler);
        }

        public void RemoveAll()
        {
            m_filler = 0;
        }

        public void Resize(int value, ResizeType resizeType = ResizeType.NewValue)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");

            switch (resizeType)
            {
                case ResizeType.NewValue:
                    m_threshold = value;
                    m_filler = m_filler.Clamp(0, m_threshold);
                    break;

                case ResizeType.Increase:
                    m_threshold += value;
                    break;

                case ResizeType.Decrease:
                    m_threshold -= value.Clamp(0, m_threshold);
                    m_filler = m_filler.Clamp(0, m_threshold);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is FilledInt && Equals((FilledInt)obj);
        }

        public bool Equals(FilledInt other)
        {
            return m_filler == other.m_filler && m_threshold == other.m_threshold;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_filler.GetHashCode(), m_threshold.GetHashCode());
        }
    }
}
