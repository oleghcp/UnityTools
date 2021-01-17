using System;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

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
            get { return (float)m_filler.CutAfter(m_threshold) / m_threshold; }
        }

        public int Excess
        {
            get { return (m_filler - m_threshold).CutBefore(0); }
        }

        public FilledInt(int threshold)
        {
            if (threshold < 0)
                throw Errors.NegativeParameter(nameof(threshold));

            m_threshold = threshold;
            m_filler = 0;
        }

        public void Fill(int addValue)
        {
            if (addValue < 0)
                throw Errors.NegativeParameter(nameof(addValue));

            m_filler += addValue;
        }

        public void FillFully()
        {
            if (m_filler < m_threshold)
                m_filler = m_threshold;
        }

        public void Remove(int removeValue)
        {
            if (removeValue < 0)
                throw Errors.NegativeParameter(nameof(removeValue));

            m_filler -= removeValue.CutAfter(m_filler);
        }

        public void RemoveAll()
        {
            m_filler = 0;
        }

        public void RemoveTillExcess()
        {
            m_filler = Excess;
        }

        public void Resize(int value, ResizeType resizeType = ResizeType.NewValue)
        {
            switch (resizeType)
            {
                case ResizeType.NewValue:
                    if (value < 0)
                        throw Errors.NegativeParameter(nameof(value));
                    m_threshold = value;
                    break;

                case ResizeType.Delta:
                    m_threshold = (m_threshold + value).CutBefore(0);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is FilledInt other && Equals(other);
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
