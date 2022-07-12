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
            set
            {
                if (value < 0f)
                    throw Errors.NegativeParameter(nameof(Threshold));

                _threshold = value;
            }
        }

        public float CurValue => _filler.CutAfter(_threshold);
        public bool FilledFully => _filler >= _threshold;
        public bool IsEmpty => _filler == 0f;
        public float Ratio => CurValue / _threshold;
        public float Excess => (_filler - _threshold).CutBefore(0f);
        public float Shortage => (_threshold - _filler).Clamp(0f, _threshold);

        public FilledFloat(float threshold)
        {
            if (threshold < 0f)
                throw Errors.NegativeParameter(nameof(threshold));

            _threshold = threshold;
            _filler = 0f;
        }

        public void Fill(float delta)
        {
            if (delta < 0f)
                throw Errors.NegativeParameter(nameof(delta));

            _filler += delta;
        }

        public void FillFully()
        {
            if (_filler < _threshold)
                _filler = _threshold;
        }

        public void Remove(float delta)
        {
            if (delta < 0f)
                throw Errors.NegativeParameter(nameof(delta));

            _filler = _filler.CutAfter(_threshold);
            _filler -= delta.CutAfter(_filler);
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
