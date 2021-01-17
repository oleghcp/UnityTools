using System;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct FilledFloat : IFilledEntity<float>, IEquatable<FilledFloat>
    {
        [SerializeField, HideInInspector]
        private float m_threshold;
        [SerializeField, HideInInspector]
        private float m_filler;

        public float CurValue
        {
            get { return m_filler.Clamp(0f, m_threshold); }
        }

        public float Threshold
        {
            get { return m_threshold; }
        }

        public bool FilledFully
        {
            get { return m_filler >= m_threshold; }
        }

        public bool IsEmpty
        {
            get { return m_filler == 0f; }
        }

        public float Ratio
        {
            get { return m_filler.CutAfter(m_threshold) / m_threshold; }
        }

        public float Excess
        {
            get { return (m_filler - m_threshold).CutBefore(0f); }
        }

        public FilledFloat(float threshold)
        {
            if (threshold < 0f)
                throw Errors.NegativeParameter(nameof(threshold));

            m_threshold = threshold;
            m_filler = 0f;
        }

        public void Fill(float addValue)
        {
            if (addValue < 0f)
                throw Errors.NegativeParameter(nameof(addValue));

            m_filler += addValue;
        }

        public void FillFully()
        {
            if (m_filler < m_threshold)
                m_filler = m_threshold;
        }

        public void Remove(float removeValue)
        {
            if (removeValue < 0f)
                throw Errors.NegativeParameter(nameof(removeValue));

            m_filler -= removeValue.CutAfter(m_filler);
        }

        public void RemoveAll()
        {
            m_filler = 0f;
        }

        public void RemoveTillExcess()
        {
            m_filler = Excess;
        }

        public void Resize(float value, ResizeType resizeType = ResizeType.NewValue)
        {
            if (value < 0f)
                throw Errors.NegativeParameter(nameof(value));

            switch (resizeType)
            {
                case ResizeType.NewValue:
                    m_threshold = value;
                    break;

                case ResizeType.Delta:
                    m_threshold = (m_threshold + value).CutBefore(0f);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is FilledFloat other && Equals(other);
        }

        public bool Equals(FilledFloat other)
        {
            return m_filler == other.m_filler && m_threshold == other.m_threshold;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(m_filler.GetHashCode(), m_threshold.GetHashCode());
        }
    }
}
