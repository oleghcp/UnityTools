using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OlegHcp.Mathematics;
using OlegHcp.Tools;

namespace OlegHcp.NumericEntities
{
    [Serializable]
    public sealed class ModifiableInt : IModifiableEntity<int>
    {
        private bool _cachingModifiedValue;
        private int _min;
        private int _max;
        private int _pureValue;
        private int _modifiedValue;
        private HashSet<IModifier<int>> _modifiers;

        public int PureValue
        {
            get => _pureValue;
            set => _pureValue = value.Clamp(_min, _max);
        }

        public bool CachingModifiedValue
        {
            get => _cachingModifiedValue;
            set
            {
                if (value && _cachingModifiedValue != value)
                    InitModifiedValue();

                _cachingModifiedValue = value;
            }
        }

        public int MinValue => _min;
        public int MaxValue => _max;
        public bool Modified => _modifiers.Count > 0;

        public ModifiableInt(int pureValue, int minValue, int maxValue, bool cachingModifiedValue)
        {
            if (minValue > maxValue)
                throw ThrowErrors.MinMax(nameof(minValue), nameof(maxValue));

            _cachingModifiedValue = cachingModifiedValue;
            _min = minValue;
            _max = maxValue;
            _pureValue = pureValue.Clamp(minValue, maxValue);

            _modifiers = new HashSet<IModifier<int>>();
        }

        public ModifiableInt(int pureValue, int minValue, int maxValue) : this(pureValue, minValue, maxValue, true)
        {

        }

        public ModifiableInt(int pureValue, bool cachingModifiedValue) : this(pureValue, int.MinValue, int.MaxValue, cachingModifiedValue)
        {

        }

        public ModifiableInt(int pureValue) : this(pureValue, true)
        {

        }

        public void AddModifier(IModifier<int> modifier)
        {
            if (_modifiers.Add(modifier))
            {
                if (_cachingModifiedValue)
                    InitModifiedValue();

                return;
            }

            throw ThrowErrors.ContainsModifier();
        }

        public void RemoveModifier(IModifier<int> modifier)
        {
            if (_modifiers.Remove(modifier) && _cachingModifiedValue)
                InitModifiedValue();
        }

        public int GetModifiedValue()
        {
            if (_modifiers.Count == 0)
                return _pureValue;

            if (_cachingModifiedValue)
                return _modifiedValue.Clamp(_min, _max);

            return (_pureValue + CalculateSum()).Clamp(_min, _max);
        }

        public void Resize(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw ThrowErrors.MinMax(nameof(minValue), nameof(maxValue));

            _min = minValue;
            _max = maxValue;

            _pureValue = _pureValue.Clamp(minValue, maxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitModifiedValue()
        {
            _modifiedValue = _pureValue + CalculateSum();
        }

        private int CalculateSum()
        {
            float sum = 0f;

            foreach (IModifier<int> modifier in _modifiers)
            {
                switch (modifier.Modification)
                {
                    case ModifierType.PureAdditive:
                        sum += modifier.Value;
                        break;

                    case ModifierType.RelativeMultiplier:
                        sum += _pureValue * modifier.Value;
                        break;

                    default:
                        throw new UnsupportedValueException(modifier.Modification);
                }
            }

            return sum.ToInt(RoundingWay.Floor);
        }
    }
}
