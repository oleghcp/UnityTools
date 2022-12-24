using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityUtility.Mathematics;
using UnityUtility.Tools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public sealed class ModifiableFloat : IModifiableEntity<float>
    {
        private bool _cachingModifiedValue;
        private float _min;
        private float _max;
        private float _pureValue;
        private float _modifiedValue;
        private HashSet<IModifier<float>> _modifiers;

        public float PureValue
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

        public float MinValue => _min;
        public float MaxValue => _max;
        public bool Modified => _modifiers.Count > 0;

        public ModifiableFloat(float pureValue, float minValue, float maxValue, bool cachingModifiedValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            _cachingModifiedValue = cachingModifiedValue;
            _min = minValue;
            _max = maxValue;
            _pureValue = pureValue.Clamp(minValue, maxValue);

            _modifiers = new HashSet<IModifier<float>>();
        }

        public ModifiableFloat(float pureValue, float minValue, float maxValue) : this(pureValue, minValue, maxValue, true)
        {

        }

        public ModifiableFloat(float pureValue, bool cachingModifiedValue) : this(pureValue, float.NegativeInfinity, float.PositiveInfinity, cachingModifiedValue)
        {

        }

        public ModifiableFloat(float pureValue) : this(pureValue, true)
        {

        }

        public void AddModifier(IModifier<float> modifier)
        {
            if (_modifiers.Add(modifier))
            {
                if (_cachingModifiedValue)
                    InitModifiedValue();

                return;
            }

            throw Errors.ContainsModifier();
        }

        public void RemoveModifier(IModifier<float> modifier)
        {
            if (_modifiers.Remove(modifier) && _cachingModifiedValue)
                InitModifiedValue();
        }

        public float GetModifiedValue()
        {
            if (_modifiers.Count == 0)
                return _pureValue;

            if (_cachingModifiedValue)
                return _modifiedValue.Clamp(_min, _max);

            return (_pureValue + CalculateSum()).Clamp(_min, _max);
        }

        public void Resize(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            _min = minValue;
            _max = maxValue;

            _pureValue = _pureValue.Clamp(minValue, maxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitModifiedValue()
        {
            _modifiedValue = _pureValue + CalculateSum();
        }

        private float CalculateSum()
        {
            float sum = 0f;

            foreach (IModifier<float> modifier in _modifiers)
            {
                switch (modifier.Modification)
                {
                    case ModifierType.PureAdditive:
                        sum += modifier.Value;
                        break;

                    case ModifierType.RelativeMultiplier:
                        sum += _pureValue * modifier.Value;
                        break;

                    case ModifierType.RelativeDivider:
                        sum += _pureValue / modifier.Value;
                        break;

                    default:
                        throw new UnsupportedValueException(modifier.Modification);
                }
            }

            return sum;
        }
    }
}
