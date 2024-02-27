using System;
using UnityEngine;
using OlegHcp.Rng;

namespace OlegHcp.NumericEntities
{
    [Serializable]
    public struct RngParam
    {
        [SerializeField]
        private Diapason _range;
        [SerializeField]
        private Option _params;

        public Diapason Range { get => _range; set => _range = value; }
        public RngMode Mode { get => _params.Mode; set => _params.Mode = value; }
        public float Intensity { get => _params.Intensity; set => _params.Intensity = value; }

#if UNITY_EDITOR
        internal static string RangeFieldName = nameof(_range);
        internal static string ParamsFieldName = nameof(_params);
#endif

        public RngParam(in Diapason range)
        {
            _range = range;
            _params = new Option();
        }

        public RngParam(in Diapason range, RngMode mode, float intensity = 1f)
        {
            _range = range;
            _params = new Option(mode, intensity);
        }

        public float GetRandomValue()
        {
            return GetRandomValue(RandomNumberGenerator.Default);
        }

        public float GetRandomValue(IRng rng)
        {
            switch (_params.Mode)
            {
                case RngMode.Simple: return rng.Next(_range);
                case RngMode.Ascending: return rng.Ascending(_range, _params.Intensity);
                case RngMode.Descending: return rng.Descending(_range, _params.Intensity);
                case RngMode.MinMax: return rng.MinMax(_range, _params.Intensity);
                case RngMode.Average: return rng.Average(_range, _params.Intensity);
                default: throw new UnsupportedValueException(_params.Mode);
            }
        }

        public static implicit operator RngParam(Diapason value)
        {
            return new RngParam(value);
        }

        public static implicit operator RngParam((float, float) value)
        {
            return new RngParam(value);
        }

        public enum RngMode
        {
            Simple,
            Ascending,
            Descending,
            MinMax,
            Average,
        }

        [Serializable]
        internal struct Option
        {
            [SerializeField]
            private RngMode _mode;
            [SerializeField, Min(0f)]
            private float _intensity;

            public RngMode Mode { get => _mode; set => _mode = value; }
            public float Intensity { get => _intensity; set => _intensity = value; }

#if UNITY_EDITOR
            internal static string ModeFieldName = nameof(_mode);
            internal static string IntensityFieldName = nameof(_intensity);
#endif

            public Option(RngMode mehod, float intensity)
            {
                _mode = mehod;
                _intensity = intensity;
            }
        }
    }
}
