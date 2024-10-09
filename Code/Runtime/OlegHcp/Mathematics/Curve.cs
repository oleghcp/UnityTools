using System;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.Mathematics
{
    [Serializable]
    public abstract class Curve<TVector>
    {
        [SerializeField]
        private TVector[] _points;

        public int Count => _points.Length;
        public ref TVector this[int index] => ref _points[index];
        protected TVector[] Points => _points;

        public Curve(TVector[] points, int requiredCount)
        {
            if (points == null)
                throw ThrowErrors.NullParameter(nameof(points));

            if (points.Length < requiredCount)
                throw ThrowErrors.InvalidCurvePoints(nameof(points), requiredCount);

            _points = points;
        }

        public abstract TVector Evaluate(float ratio);
    }
}
