using System;
using System.Collections.Generic;
using UnityUtility.Mathematics;
using UnityUtilityTools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public sealed class StaticFloat : IStaticEntity<float>
    {
        private float _min;
        private float _max;
        private float _value;

        private HashSet<IModifier<float>> _absMods;
        private HashSet<IModifier<float>> _relMods;

        public float PureValue
        {
            get => _value;
            set => _value = value.Clamp(_min, _max);
        }

        public float MinValue => _min;
        public float MaxValue => _max;
        public bool Modified => _absMods.Count > 0 && _relMods.Count > 0;

        private StaticFloat(float minValue, float maxValue)
        {
            Resize(minValue, maxValue);

            _absMods = new HashSet<IModifier<float>>();
            _relMods = new HashSet<IModifier<float>>();
        }

        public StaticFloat(float pureValue, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity) : this(minValue, maxValue)
        {
            if (pureValue < minValue || pureValue > maxValue)
                throw Errors.OutOfRange(nameof(pureValue), nameof(minValue), nameof(maxValue));

            _value = pureValue;
        }

        public void AddModifier(IModifier<float> modifier)
        {
            HashSet<IModifier<float>> collection = modifier.Relative ? _relMods : _absMods;

            if (collection.Add(modifier))
                return;

            throw Errors.ContainsModifier();
        }

        public void RemoveModifier(IModifier<float> modifier)
        {
            if (modifier.Relative)
                _relMods.Remove(modifier);
            else
                _absMods.Remove(modifier);
        }

        public float GetModifiedValue()
        {
            return (_value + GetAbsSum() + GetRelSum()).Clamp(_min, _max);
        }

        public void Resize(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            _min = minValue;
            _max = maxValue;

            _value = _value.Clamp(minValue, maxValue);
        }

        //--//

        private float GetAbsSum()
        {
            if (_absMods.Count == 0)
                return 0f;

            float sum = 0f;

            foreach (IModifier<float> item in _absMods)
            {
                sum += item.Value;
            }

            return sum;
        }

        private float GetRelSum()
        {
            if (_relMods.Count == 0)
                return 0f;

            float sum = 0f;
            float value = _value;

            foreach (IModifier<float> item in _relMods)
            {
                sum += value * item.Value;
            }

            return sum;
        }
    }
}
