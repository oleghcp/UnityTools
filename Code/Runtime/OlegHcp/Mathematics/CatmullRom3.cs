using System;
using System.Linq;
using OlegHcp.CSharp;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.Mathematics
{
    [Serializable]
    public class CatmullRom3 : ICurve<Vector3>
    {
        internal const int REQUIRED_COUNT = 4;

        private Vector3[] _points;
        private float _alpha;
        [NonSerialized]
        private float[] _weights;
        [NonSerialized]
        private bool _weightsInitialized;

        public int Count => _points.Length;

        public Vector3 this[int index]
        {
            get => _points[index];
            set
            {
                if (_points[index] == value)
                    return;

                _points[index] = value;
                _weightsInitialized = false;
            }
        }

        public float Alpha
        {
            get => _alpha;
            set
            {
                if (_alpha == value)
                    return;

                _alpha = value.Clamp01();
                _weightsInitialized = false;
            }
        }

        public CatmullRom3(Vector3[] points, float alpha = 0.5f)
        {
            if (points == null)
                throw ThrowErrors.NullParameter(nameof(points));

            if (points.Length < REQUIRED_COUNT)
                throw ThrowErrors.InvalidCurvePoints(nameof(points), REQUIRED_COUNT);

            _alpha = alpha.Clamp01();
            _points = points;
        }

        public Vector3 Evaluate(float ratio)
        {
            if (ratio <= 0f)
                return _points[1];

            if (ratio >= 1f)
                return _points.FromEnd(1);

            if (!_weightsInitialized)
            {
                if (_weights == null)
                {
                    if (_points.Length > REQUIRED_COUNT)
                        _weights = new float[_points.Length - 3];
                    else
                        _weights = Array.Empty<float>();
                }

                RecalculateWeights(_points, _weights, _alpha);
                _weightsInitialized = true;
            }

            int weightIndex = 0;

            float sum = 0f;
            for (int i = 0; i < _weights.Length; i++)
            {
                if (ratio > sum + _weights[i])
                {
                    sum += _weights[i];
                    continue;
                }

                ratio = (ratio - sum) / _weights[i];
                weightIndex = i;
                break;
            }

            return EvaluateFourPoints(_points[weightIndex], _points[weightIndex + 1], _points[weightIndex + 2], _points[weightIndex + 3], _alpha, ratio);
        }

        public static Vector3 Evaluate(in Vector3 point0, in Vector3 point1, in Vector3 point2, in Vector3 point3, float alpha, float ratio)
        {
            if (ratio <= 0f)
                return point1;

            if (ratio >= 1f)
                return point2;

            return EvaluateFourPoints(point0, point1, point2, point3, alpha.Clamp01(), ratio);
        }

        private static Vector3 EvaluateFourPoints(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3, float alpha, float ratio)
        {
            float pow = alpha * 0.5f;

            float t0 = 0f;
            float t1 = Vector3.SqrMagnitude(point0 - point1).Pow(pow);
            float t2 = Vector3.SqrMagnitude(point1 - point2).Pow(pow) + t1;
            float t3 = Vector3.SqrMagnitude(point2 - point3).Pow(pow) + t2;
            float t = Mathf.LerpUnclamped(t1, t2, ratio);

            point0 = Vector3.LerpUnclamped(point0, point1, (t - t0) / (t1 - t0));
            point1 = Vector3.LerpUnclamped(point1, point2, (t - t1) / (t2 - t1));
            point2 = Vector3.LerpUnclamped(point2, point3, (t - t2) / (t3 - t2));

            point0 = Vector3.LerpUnclamped(point0, point1, (t - t0) / (t2 - t0));
            point1 = Vector3.LerpUnclamped(point1, point2, (t - t1) / (t3 - t1));

            return Vector3.LerpUnclamped(point0, point1, (t - t1) / (t2 - t1));
        }

        private static void RecalculateWeights(Vector3[] points, float[] weights, float alpha)
        {
            if (weights.Length == 0)
                return;

            float sum = 0f;
            for (int i = 0; i < weights.Length; i++)
            {
                Vector3 start = points[i + 1];
                Vector3 stepped = EvaluateFourPoints(points[i], points[i + 1], points[i + 2], points[i + 3], alpha, 0.1f);
                float distance = Vector3.Distance(start, stepped);
                weights[i] = distance;
                sum += distance;
            }

            float normalizeFactor = 1f / sum;

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] *= normalizeFactor;
            }
        }
    }
}
