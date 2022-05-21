using System;
using System.Collections.Generic;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public sealed class StaticInt : IStaticEntity<int>
    {
        private int _min;
        private int _max;
        private int _value;

        private HashSet<IAbsoluteModifier<int>> _absMods;
        private HashSet<IRelativeModifier<int>> _relMods;

        public int PureValue => _value;
        public int MinValue => _min;
        public int MaxValue => _max;
        public bool Modified => _absMods.Count > 0 && _relMods.Count > 0;

        private StaticInt(int minValue, int maxValue)
        {
            Resize(minValue, maxValue);

            _absMods = new HashSet<IAbsoluteModifier<int>>();
            _relMods = new HashSet<IRelativeModifier<int>>();
        }

        public StaticInt(int pureValue, int minValue = int.MinValue, int maxValue = int.MaxValue) : this(minValue, maxValue)
        {
            if (pureValue < minValue || pureValue > maxValue)
                throw Errors.OutOfRange(nameof(pureValue), nameof(minValue), nameof(maxValue));

            _value = pureValue;
        }

        public void AddModifier(IAbsoluteModifier<int> modifier)
        {
            _absMods.Add(modifier);
        }

        public void AddModifier(IRelativeModifier<int> modifier)
        {
            _relMods.Add(modifier);
        }

        public void RemoveModifier(IAbsoluteModifier<int> modifier)
        {
            _absMods.Remove(modifier);
        }

        public void RemoveModifier(IRelativeModifier<int> modifier)
        {
            _relMods.Remove(modifier);
        }

        public int GetCurValue()
        {
            return (_value + GetAbsSum() + GetRelSum()).Clamp(_min, _max);
        }

        public void Revalue(int value, ResizeType resizeType = ResizeType.NewValue)
        {
            switch (resizeType)
            {
                case ResizeType.NewValue:
                    if (value < _min || value > _max)
                        throw Errors.OutOfRange(nameof(value), nameof(MinValue), nameof(MaxValue));
                    _value = value;
                    break;

                case ResizeType.Delta:
                    _value = (_value + value).Clamp(_min, _max);
                    break;

                default:
                    throw new UnsupportedValueException(resizeType);
            }
        }

        public void Resize(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            _min = minValue;
            _max = maxValue;

            _value = _value.Clamp(_min, _max);
        }

        //--//

        private int GetAbsSum()
        {
            if (_absMods.Count == 0)
                return 0;

            int sum = 0;

            foreach (var item in _absMods)
            {
                sum += item.Value;
            }

            return sum;
        }

        private int GetRelSum()
        {
            if (_relMods.Count == 0)
                return 0;

            int sum = 0;

            foreach (var item in _relMods)
            {
                sum += _value * item.Value;
            }

            return sum;
        }
    }
}
