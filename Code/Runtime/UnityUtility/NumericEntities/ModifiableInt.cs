using System;
using System.Collections.Generic;
using UnityUtility.Mathematics;
using UnityUtility.Tools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public sealed class ModifiableInt : IModifiableEntity<int>
    {
        private int _min;
        private int _max;
        private int _value;

        private HashSet<IModifier<int>> _absMods;
        private HashSet<IModifier<int>> _relMods;

        public int PureValue
        {
            get => _value;
            set => _value = value.Clamp(_min, _max);
        }

        public int MinValue => _min;
        public int MaxValue => _max;
        public bool Modified => _absMods.Count > 0 && _relMods.Count > 0;

        private ModifiableInt(int minValue, int maxValue)
        {
            Resize(minValue, maxValue);

            _absMods = new HashSet<IModifier<int>>();
            _relMods = new HashSet<IModifier<int>>();
        }

        public ModifiableInt(int pureValue, int minValue = int.MinValue, int maxValue = int.MaxValue) : this(minValue, maxValue)
        {
            if (pureValue < minValue || pureValue > maxValue)
                throw Errors.OutOfRange(nameof(pureValue), nameof(minValue), nameof(maxValue));

            _value = pureValue;
        }

        public void AddModifier(IModifier<int> modifier)
        {
            HashSet<IModifier<int>> collection = modifier.Relative ? _relMods : _absMods;

            if (collection.Add(modifier))
                return;

            throw Errors.ContainsModifier();
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

        public void Resize(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            _min = minValue;
            _max = maxValue;

            _value = _value.Clamp(minValue, maxValue);
        }

        //--//

        private int GetAbsSum()
        {
            if (_absMods.Count == 0)
                return 0;

            int sum = 0;

            foreach (IModifier<int> item in _absMods)
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

            foreach (IModifier<int> item in _relMods)
            {
                sum += _value * item.Value;
            }

            return sum;
        }
    }
}
