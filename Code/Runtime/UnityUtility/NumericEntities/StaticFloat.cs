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

        private HashSet<IAbsoluteModifier<float>> _absMods;
        private HashSet<IRelativeModifier<float>> _relMods;
        private Func<float> _getValue;

        public float PureValue => _getValue();
        public float MinValue => _min;
        public float MaxValue => _max;
        public bool Modified => _absMods.Count > 0 && _relMods.Count > 0;

        private StaticFloat(float minValue, float maxValue)
        {
            Resize(minValue, maxValue);

            _absMods = new HashSet<IAbsoluteModifier<float>>();
            _relMods = new HashSet<IRelativeModifier<float>>();
        }

        public StaticFloat(float pureValue, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity) : this(minValue, maxValue)
        {
            if (pureValue < minValue || pureValue > maxValue)
                throw Errors.OutOfRange(nameof(pureValue), nameof(minValue), nameof(maxValue));

            _value = pureValue;
            _getValue = () => _value;
        }

        public StaticFloat(Func<float> valueDefiner, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity) : this(minValue, maxValue)
        {
            if (valueDefiner == null)
                throw new ArgumentNullException(nameof(valueDefiner));

            _getValue = valueDefiner;
        }

        public void AddModifier(IAbsoluteModifier<float> modifier)
        {
            _absMods.Add(modifier);
        }

        public void AddModifier(IRelativeModifier<float> modifier)
        {
            _relMods.Add(modifier);
        }

        public void RemoveModifier(IAbsoluteModifier<float> modifier)
        {
            _absMods.Remove(modifier);
        }

        public void RemoveModifier(IRelativeModifier<float> modifier)
        {
            _relMods.Remove(modifier);
        }

        public float GetCurValue()
        {
            return (_getValue() + GetAbsSum() + GetRelSum()).Clamp(_min, _max);
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
            float value = _getValue();

            foreach (var item in _relMods)
            {
                sum += value * item.Value;
            }

            return sum;
        }
    }
}
