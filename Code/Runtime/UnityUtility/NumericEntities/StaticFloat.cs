using System;
using System.Collections.Generic;
using UnityUtility.MathExt;
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

        public float PureValue => _value;
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
            if (modifier.Relative)
                _relMods.Add(modifier);
            else
                _absMods.Add(modifier);
        }

        public void RemoveModifier(IModifier<float> modifier)
        {
            var collection = modifier.Relative ? _relMods : _absMods;

            if (collection.Add(modifier))
                return;

            throw new InvalidOperationException("Modifier already added.");
        }

        public float GetModifiedValue()
        {
            return (_value + GetAbsSum() + GetRelSum()).Clamp(_min, _max);
        }

        public void Revalue(float value, ResizeType resizeType = ResizeType.NewValue)
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

        public void Resize(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            _min = minValue;
            _max = maxValue;

            _value = _value.Clamp(_min, _max);
        }

        //--//

        private float GetAbsSum()
        {
            if (_absMods.Count == 0)
                return 0f;

            float sum = 0f;

            foreach (var item in _absMods)
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

            foreach (var item in _relMods)
            {
                sum += value * item.Value;
            }

            return sum;
        }
    }
}
