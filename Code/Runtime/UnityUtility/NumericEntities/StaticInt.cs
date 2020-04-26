using UnityUtility.MathExt;
using System;
using System.Collections.Generic;

namespace UnityUtility.NumericEntities
{
    public sealed class StaticInt : IStaticEntity<int>
    {
        private int m_min;
        private int m_max;
        private int m_value;

        private HashSet<IAbsoluteModifier<int>> m_absMods;
        private HashSet<IRelativeModifier<int>> m_relMods;
        private Func<int> m_getValue;

        public int PureValue
        {
            get { return m_getValue(); }
        }

        public int MinValue
        {
            get { return m_min; }
        }

        public int MaxValue
        {
            get { return m_max; }
        }

        public bool Modified
        {
            get { return m_absMods.Count > 0 && m_relMods.Count > 0; }
        }

        private StaticInt(int minValue, int maxValue)
        {
            Resize(minValue, maxValue);

            m_absMods = new HashSet<IAbsoluteModifier<int>>();
            m_relMods = new HashSet<IRelativeModifier<int>>();
        }

        public StaticInt(int pureValue, int minValue = int.MinValue, int maxValue = int.MaxValue) : this(minValue, maxValue)
        {
            if (pureValue < minValue || pureValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(pureValue), "pureValue cannot be out of range between minValue and maxValue.");

            m_value = pureValue;
            m_getValue = () => m_value;
        }

        public StaticInt(Func<int> valueDefiner, int minValue = int.MinValue, int maxValue = int.MaxValue) : this(minValue, maxValue)
        {
            if (valueDefiner == null)
                throw new ArgumentNullException(nameof(valueDefiner));

            m_getValue = valueDefiner;
        }

        public void AddModifier(IAbsoluteModifier<int> modifier)
        {
            m_absMods.Add(modifier);
        }

        public void AddModifier(IRelativeModifier<int> modifier)
        {
            m_relMods.Add(modifier);
        }

        public void RemoveModifier(IAbsoluteModifier<int> modifier)
        {
            m_absMods.Remove(modifier);
        }

        public void RemoveModifier(IRelativeModifier<int> modifier)
        {
            m_relMods.Remove(modifier);
        }

        public int GetCurValue()
        {
            return (m_getValue() + f_getAbsSum() + f_getRelSum()).Clamp(m_min, m_max);
        }

        public void Revalue(int value, ResizeType resizeType = ResizeType.NewValue)
        {
            switch (resizeType)
            {
                case ResizeType.NewValue:
                    if (value < m_min || value > m_max)
                        throw new ArgumentOutOfRangeException(nameof(value), "pureValue cannot be out of range between MinValue and MaxValue.");
                    m_value = value;
                    break;

                case ResizeType.Increase:
                    if (value < 0)
                        throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");
                    m_value += value.Clamp(0, m_max);
                    break;

                case ResizeType.Decrease:
                    if (value < 0)
                        throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero.");
                    m_value -= value.Clamp(0, m_value - m_min);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        public void Resize(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), "minValue cannot be more than maxValue.");

            m_min = minValue;
            m_max = maxValue;

            m_value = m_value.Clamp(m_min, m_max);
        }

        //--//

        private int f_getAbsSum()
        {
            if (m_absMods.Count == 0)
                return 0;

            int sum = 0;

            foreach (var item in m_absMods)
            {
                sum += item.Value;
            }

            return sum;
        }

        private int f_getRelSum()
        {
            if (m_relMods.Count == 0)
                return 0;

            int sum = 0;
            int value = m_getValue();

            foreach (var item in m_relMods)
            {
                sum += value * item.Multiplier;
            }

            return sum;
        }
    }
}
