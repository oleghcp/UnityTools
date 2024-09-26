using System;
using OlegHcp.Mathematics;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp
{
#if UNITY
    [Serializable]
#endif
    public struct IntervalChecker
    {
#if UNITY
        [SerializeField]
#endif
        private float _interval;
#if UNITY
        [SerializeField]
#endif
        private float _currentValue;

        public float Interval
        {
            get => _interval;
            set
            {
                if (value <= 0f)
                    throw ThrowErrors.ZeroParameter(nameof(value));

                _interval = value;
            }
        }

        public float CurrentValue
        {
            get => _currentValue;
            set => _currentValue = value.ClampMax(_interval);
        }

        public float Ratio
        {
            get => _currentValue / _interval;
            set => _currentValue = _interval * value.Clamp01();
        }

        public float Shortage => (_interval - _currentValue).ClampMin(0f);

        public IntervalChecker(float interval)
        {
            if (interval < 0f)
                throw ThrowErrors.NegativeParameter(nameof(interval));

            _interval = interval;
            _currentValue = 0f;
        }

        public IntervalChecker(float interval, float startRatio)
        {
            if (interval < 0f)
                throw ThrowErrors.NegativeParameter(nameof(interval));

            _interval = interval;
            _currentValue = _interval * startRatio.Clamp01();
        }

        public bool HardCheckDelta(float deltaValue)
        {
            if (deltaValue < 0f)
                throw ThrowErrors.NegativeParameter(nameof(deltaValue));

            return HardCheckValue(_currentValue + deltaValue);
        }

        public bool HardCheckValue(float newValue)
        {
            if (newValue >= _interval)
            {
                _currentValue = 0f;
                return true;
            }

            _currentValue = newValue;
            return false;
        }

        public bool SmoothCheckDelta(float deltaValue)
        {
            if (deltaValue < 0f)
                throw ThrowErrors.NegativeParameter(nameof(deltaValue));

            return SmoothCheckValue(_currentValue + deltaValue);
        }

        public bool SmoothCheckValue(float newValue)
        {
            if (newValue >= _interval)
            {
                _currentValue = newValue - _interval;
                return true;
            }

            _currentValue = newValue;
            return false;
        }
    }
}
