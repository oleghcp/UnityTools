using System;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct FilledFloat : IFilledEntity<float>, IEquatable<FilledFloat>
    {
        [SerializeField, HideInInspector]
        private float _threshold;
        [SerializeField, HideInInspector]
        private float _filler;

        public float Threshold
        {
            get => _threshold;
            set => _threshold = value.CutBefore(0f);
        }

        public float CurValue => _filler.Clamp(0f, _threshold);
        public bool FilledFully => _filler >= _threshold;
        public bool IsEmpty => _filler == 0f;
        public float Ratio => _filler.CutAfter(_threshold) / _threshold;
        public float Excess => (_filler - _threshold).CutBefore(0f);

        public FilledFloat(float threshold)
        {
            if (threshold < 0f)
                throw Errors.NegativeParameter(nameof(threshold));

            _threshold = threshold;
            _filler = 0f;
        }

        public void Fill(float addValue)
        {
            if (addValue < 0f)
                throw Errors.NegativeParameter(nameof(addValue));

            _filler += addValue;
        }

        public void FillFully()
        {
            if (_filler < _threshold)
                _filler = _threshold;
        }

        public void Remove(float removeValue)
        {
            if (removeValue < 0f)
                throw Errors.NegativeParameter(nameof(removeValue));

            _filler -= removeValue.CutAfter(_filler);
        }

        public void RemoveAll()
        {
            _filler = 0f;
        }

        public void RemoveTillExcess()
        {
            _filler = Excess;
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is FilledFloat other && Equals(other);
        }

        public bool Equals(FilledFloat other)
        {
            return _filler == other._filler && _threshold == other._threshold;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(_filler.GetHashCode(), _threshold.GetHashCode());
        }
    }
}
