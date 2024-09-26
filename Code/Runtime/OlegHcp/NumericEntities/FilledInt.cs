using System;
using OlegHcp.Mathematics;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.NumericEntities
{
#if UNITY
    [Serializable]
#endif
    public struct FilledInt : IFilledEntity<int>, IEquatable<FilledInt>
    {
#if UNITY
        [SerializeField, HideInInspector]
#endif
        private int _threshold;
#if UNITY
        [SerializeField, HideInInspector]
#endif
        private int _filler;

        public int Threshold
        {
            get => _threshold;
            set
            {
                if (value < 0)
                    throw ThrowErrors.NegativeParameter(nameof(Threshold));

                _threshold = value;
            }
        }

        public int CurValue => _filler.ClampMax(_threshold);
        public bool FilledFully => _filler >= _threshold;
        public bool IsEmpty => _filler == 0;
        public float Ratio => NumericHelper.GetRatio(CurValue, _threshold);
        public int Excess => (_filler - _threshold).ClampMin(0);
        public int Shortage => (_threshold - _filler).Clamp(0, _threshold);

#if UNITY_EDITOR
        internal static string ThresholdFieldName => nameof(_threshold);
        internal static string FillerFieldName => nameof(_filler);
#endif

        public FilledInt(int threshold)
        {
            if (threshold < 0)
                throw ThrowErrors.NegativeParameter(nameof(threshold));

            _threshold = threshold;
            _filler = 0;
        }

        public void Fill(int delta)
        {
            if (delta < 0)
                throw ThrowErrors.NegativeParameter(nameof(delta));

            _filler += delta;
        }

        public void FillFully()
        {
            if (_filler < _threshold)
                _filler = _threshold;
        }

        public void Remove(int delta)
        {
            if (delta < 0)
                throw ThrowErrors.NegativeParameter(nameof(delta));

            _filler = _filler.ClampMax(_threshold);
            _filler -= delta.ClampMax(_filler);
        }

        public void RemoveAll()
        {
            _filler = 0;
        }

        public void RemoveExcess()
        {
            if (_filler > _threshold)
                _filler = _threshold;
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is FilledInt other && Equals(other);
        }

        public bool Equals(FilledInt other)
        {
            return _filler == other._filler && _threshold == other._threshold;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_filler, _threshold);
        }
    }
}
