using System;
using System.Collections.Generic;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public sealed class StaticFloat : IStaticEntity<float>
    {
        private float m_min;
        private float m_max;
        private float m_value;

        private HashSet<IAbsoluteModifier<float>> m_absMods;
        private HashSet<IRelativeModifier<float>> m_relMods;
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

            m_absMods = new HashSet<IAbsoluteModifier<float>>();
            m_relMods = new HashSet<IRelativeModifier<float>>();
        }

        public StaticFloat(float pureValue, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity) : this(minValue, maxValue)
        {
            if (pureValue < minValue || pureValue > maxValue)
                throw Errors.OutOfRange(nameof(pureValue), nameof(minValue), nameof(maxValue));

            m_value = pureValue;
            m_getValue = () => m_value;
        }

        public StaticFloat(Func<float> valueDefiner, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity) : this(minValue, maxValue)
        {
            if (valueDefiner == null)
                throw new ArgumentNullException(nameof(valueDefiner));

            m_getValue = valueDefiner;
        }

        public void AddModifier(IAbsoluteModifier<float> modifier)
        {
            m_absMods.Add(modifier);
        }

        public void AddModifier(IRelativeModifier<float> modifier)
        {
            m_relMods.Add(modifier);
        }

        public void RemoveModifier(IAbsoluteModifier<float> modifier)
        {
            m_absMods.Remove(modifier);
        }

        public void RemoveModifier(IRelativeModifier<float> modifier)
        {
            m_relMods.Remove(modifier);
        }

        public float GetCurValue()
        {
            return (m_getValue() + GetAbsSum() + GetRelSum()).Clamp(m_min, m_max);
        }

        public void Revalue(float value, ResizeType resizeType = ResizeType.NewValue)
        {
            switch (resizeType)
            {
                case ResizeType.NewValue:
                    if (value < m_min || value > m_max)
                        throw Errors.OutOfRange(nameof(value), nameof(MinValue), nameof(MaxValue));
                    m_value = value;
                    break;

                case ResizeType.Delta:
                    m_value = (m_value + value).Clamp(m_min, m_max);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        public void Resize(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            m_min = minValue;
            m_max = maxValue;

            m_value = m_value.Clamp(m_min, m_max);
        }

        //--//

        private float GetAbsSum()
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

        private float GetRelSum()
        {
            if (m_relMods.Count == 0)
                return 0f;

            float sum = 0f;
            float value = m_getValue();

            foreach (var item in m_relMods)
            {
                sum += value * item.Value;
            }

            return sum;
        }
    }
}
