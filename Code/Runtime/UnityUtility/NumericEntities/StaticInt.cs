using System;
using System.Collections.Generic;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
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
                throw Errors.OutOfRange(nameof(pureValue), nameof(minValue), nameof(maxValue));

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
                        throw Errors.OutOfRange(nameof(value), nameof(MinValue), nameof(MaxValue));
                    m_value = value;
                    break;

                case ResizeType.Increase:
                    if (value < 0f)
                        throw Errors.NegativeParameter(nameof(value));
                    m_value += value.CutAfter(m_max);
                    break;

                case ResizeType.Decrease:
                    if (value < 0f)
                        throw Errors.NegativeParameter(nameof(value));
                    m_value -= value.CutAfter(m_value - m_min);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        public void Resize(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

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
                sum += value * item.Value;
            }

            return sum;
        }
    }
}
