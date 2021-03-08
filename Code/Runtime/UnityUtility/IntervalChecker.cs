using System;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility
{
    [Serializable]
    public struct IntervalChecker
    {
        [SerializeField]
        private float _interval;
        [SerializeField]
        private float _currentValue;

        public float Interval
        {
            get { return _interval; }
            set
            {
                if (value <= 0f)
                    throw Errors.ZeroParameter(nameof(value));

                _interval = value;
            }
        }

        public float CurrentValue
        {
            get { return _currentValue; }
            set { _currentValue = value.CutAfter(_interval); }
        }

        public float Ratio
        {
            get { return _currentValue / _interval; }
            set { _currentValue = _interval * value.Clamp01(); }
        }

        public float Shortage
        {
            get { return (_interval - _currentValue).CutBefore(0f); }
        }

        public IntervalChecker(float interval)
        {
            if (interval <= 0f)
                throw Errors.ZeroParameter(nameof(interval));

            _interval = interval;
            _currentValue = 0f;
        }

        public IntervalChecker(float interval, float startValue)
        {
            if (interval <= 0f)
                throw Errors.ZeroParameter(nameof(interval));

            _interval = interval;
            _currentValue = startValue.CutAfter(interval);
        }

        public bool HardCheckDelta(float deltaValue)
        {
            if (deltaValue < 0f)
                throw Errors.NegativeParameter(nameof(deltaValue));

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
                throw Errors.NegativeParameter(nameof(deltaValue));

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
