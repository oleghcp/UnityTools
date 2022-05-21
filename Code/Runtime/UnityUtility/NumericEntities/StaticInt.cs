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

        private HashSet<IModifier<int>> _absMods;
        private HashSet<IModifier<int>> _relMods;

        public int PureValue => _value;
        public int MinValue => _min;
        public int MaxValue => _max;
        public bool Modified => _absMods.Count > 0 && _relMods.Count > 0;

        private StaticInt(int minValue, int maxValue)
        {
            Resize(minValue, maxValue);

            _absMods = new HashSet<IModifier<int>>();
            _relMods = new HashSet<IModifier<int>>();
        }

        public StaticInt(int pureValue, int minValue = int.MinValue, int maxValue = int.MaxValue) : this(minValue, maxValue)
        {
            if (pureValue < minValue || pureValue > maxValue)
                throw Errors.OutOfRange(nameof(pureValue), nameof(minValue), nameof(maxValue));

            _value = pureValue;
        }

        public void AddModifier(IModifier<int> modifier)
        {
            var collection = modifier.Relative ? _relMods : _absMods;

            if (collection.Add(modifier))
                return;

            throw new InvalidOperationException("Modifier already added.");
        }

        public void RemoveModifier(IModifier<int> modifier)
        {
            if (modifier.Relative)
                _relMods.Remove(modifier);
            else
                _absMods.Remove(modifier);
        }

        public int GetModifiedValue()
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
