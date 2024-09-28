using System;
using OlegHcp.Mathematics;
using OlegHcp.Tools;

namespace OlegHcp.NumericEntities
{
#if UNITY
    [Serializable]
#endif
    public struct FilledFloat : IFilledEntity<float>, IEquatable<FilledFloat>
    {
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private float _threshold;
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private float _filler;

        public float Threshold
        {
            get => _threshold;
            set
            {
                if (value < 0f)
                    throw ThrowErrors.NegativeParameter(nameof(Threshold));

                _threshold = value;
            }
        }

        public float CurValue => _filler.ClampMax(_threshold);
        public bool FilledFully => _filler >= _threshold;
        public bool IsEmpty => _filler == 0f;
        public float Ratio => NumericHelper.GetRatio(CurValue, _threshold);
        public float Excess => (_filler - _threshold).ClampMin(0f);
        public float Shortage => (_threshold - _filler).Clamp(0f, _threshold);

#if UNITY_EDITOR
        internal static string ThresholdFieldName => nameof(_threshold);
        internal static string FillerFieldName => nameof(_filler);
#endif

        public FilledFloat(float threshold)
        {
            if (threshold < 0f)
                throw ThrowErrors.NegativeParameter(nameof(threshold));

            _threshold = threshold;
            _filler = 0f;
        }

        public void Fill(float delta)
        {
            if (delta < 0f)
                throw ThrowErrors.NegativeParameter(nameof(delta));

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
                throw ThrowErrors.NegativeParameter(nameof(delta));

            _filler = _filler.ClampMax(_threshold);
            _filler -= delta.ClampMax(_filler);
        }

        public void RemoveAll()
        {
            _filler = 0f;
        }

        public void RemoveExcess()
        {
            if (_filler > _threshold)
                _filler = _threshold;
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
            return HashCode.Combine(_filler, _threshold);
        }
    }
}
