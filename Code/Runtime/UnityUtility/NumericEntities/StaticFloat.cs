using UnityUtility.MathExt;
using System;
using System.Collections.Generic;

namespace UnityUtility.NumericEntities
{
    public sealed class StaticFloat : StaticEntity<float>
    {
        private float m_min;
        private float m_max;
        private float m_value;

        private HashSet<AbsoluteModifier<float>> m_absMods;
        private HashSet<RelativeModifier<float>> m_relMods;
        private Func<float> m_getValue;

        public float PureValue
        {
            get { return m_getValue(); }
        }

        public float MinValue
        {
            get { return m_min; }
        }

        public float MaxValue
        {
            get { return m_max; }
        }

        public bool Modified
        {
            get { return m_absMods.Count > 0 && m_relMods.Count > 0; }
        }

        private StaticFloat(float minValue, float maxValue)
        {
            Resize(minValue, maxValue);

            m_absMods = new HashSet<AbsoluteModifier<float>>();
            m_relMods = new HashSet<RelativeModifier<float>>();
        }

        public StaticFloat(float pureValue, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity) : this(minValue, maxValue)
        {
            if (pureValue < minValue || pureValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(pureValue), "pureValue cannot be out of range between minValue and maxValue.");

            m_value = pureValue;
            m_getValue = () => m_value;
        }

        public StaticFloat(Func<float> valueDefiner, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity) : this(minValue, maxValue)
        {
            if (valueDefiner == null)
                throw new ArgumentNullException(nameof(valueDefiner));

            m_getValue = valueDefiner;
        }

        public void AddModifier(AbsoluteModifier<float> modifier)
        {
            m_absMods.Add(modifier);
        }

        public void AddModifier(RelativeModifier<float> modifier)
        {
            m_relMods.Add(modifier);
        }

        public void RemoveModifier(AbsoluteModifier<float> modifier)
        {
            m_absMods.Remove(modifier);
        }

        public void RemoveModifier(RelativeModifier<float> modifier)
        {
            m_relMods.Remove(modifier);
        }

        public float GetCurValue()
        {
            return (m_getValue() + f_getAbsSum() + f_getRelSum()).Clamp(m_min, m_max);
        }

        public void Revalue(float value, ResizeType resizeType = ResizeType.NewValue)
        {
            switch (resizeType)
            {
                case ResizeType.NewValue:
                    if (value < m_min || value > m_max)
                        throw new ArgumentOutOfRangeException(nameof(value), "pureValue cannot be out of range between MinValue and MaxValue.");
                    m_value = value;
                    break;

                case ResizeType.Increase:
                    if (value < 0f)
                        throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");
                    m_value += value.Clamp(0f, m_max);
                    break;

                case ResizeType.Decrease:
                    if (value < 0f)
                        throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");
                    m_value -= value.Clamp(0f, m_value - m_min);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        public void Resize(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), "minValue cannot be more than maxValue.");

            m_min = minValue;
            m_max = maxValue;

            m_value = m_value.Clamp(m_min, m_max);
        }

        //--//

        private float f_getAbsSum()
        {
            if (m_absMods.Count == 0)
                return 0f;

            float sum = 0f;

            foreach (var item in m_absMods)
            {
                sum += item.Value;
            }

            return sum;
        }

        private float f_getRelSum()
        {
            if (m_relMods.Count == 0)
                return 0f;

            float sum = 0f;
            float value = m_getValue();

            foreach (var item in m_relMods)
            {
                sum += value * item.Multiplier;
            }

            return sum;
        }
    }
}
