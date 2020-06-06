using UnityUtility.MathExt;
using System;
using UnityEngine;
using Tools;

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
            get { return m_filler / m_threshold; }
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

            m_filler += addValue.Clamp(0f, m_threshold - m_filler);
        }

        public void FillFully()
        {
            m_filler = m_threshold;
        }

        public void Remove(float removeValue)
        {
            if (removeValue < 0f)
                throw Errors.NegativeParameter(nameof(removeValue));

            m_filler -= removeValue.Clamp(0f, m_filler);
        }

        public void RemoveAll()
        {
            m_filler = 0f;
        }

        public void Resize(float value, ResizeType resizeType = ResizeType.NewValue)
        {
            if (value < 0f)
                throw Errors.NegativeParameter(nameof(value));

            switch (resizeType)
            {
                case ResizeType.NewValue:
                    m_threshold = value;
                    m_filler = m_filler.Clamp(0f, m_threshold);
                    break;

                case ResizeType.Increase:
                    m_threshold += value;
                    break;

                case ResizeType.Decrease:
                    m_threshold -= value.Clamp(0f, m_threshold);
                    m_filler = m_filler.Clamp(0f, m_threshold);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is FilledFloat && Equals((FilledFloat)obj);
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
