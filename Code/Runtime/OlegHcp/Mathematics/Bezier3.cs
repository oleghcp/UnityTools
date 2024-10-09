using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.Mathematics
{
    [Serializable]
    public class Bezier3 : ICurve<Vector3>
    {
        internal const int REQUIRED_COUNT = 3;

        private Vector3[] _points;
        private Vector3[] _buffer;

        public int Count => _points.Length;

        public Vector3 this[int index]
        {
            get => _points[index];
            set => _points[index] = value;
        }

        public Bezier3(Vector3[] points) 
        {
            if (points == null)
                throw ThrowErrors.NullParameter(nameof(points));

            if (points.Length < REQUIRED_COUNT)
                throw ThrowErrors.InvalidCurvePoints(nameof(points), REQUIRED_COUNT);

            _points = points;
        }

        public Vector3 Evaluate(float ratio)
        {
            if (_buffer == null)
                _buffer = new Vector3[_points.Length];

            _points.CopyTo(_buffer, 0);
            return EvaluateInternal(_buffer, ratio);
        }

        public static Vector3 Evaluate(in Vector3 origin, in Vector3 destination, in Vector3 controlPoint, float ratio)
        {
            ratio = ratio.Clamp01();
            Vector3 p1 = Vector3.LerpUnclamped(origin, controlPoint, ratio);
            Vector3 p2 = Vector3.LerpUnclamped(controlPoint, destination, ratio);
            return Vector3.LerpUnclamped(p1, p2, ratio);
        }

        public static Vector3 Evaluate(Vector3[] points, float ratio)
        {
#if UNITY_2021_2_OR_NEWER || !UNITY
            return Evaluate((Span<Vector3>)points, ratio);
#else
            return Evaluate((IList<Vector3>)points, ratio);
#endif
        }

        public static Vector3 Evaluate(Span<Vector3> points, float ratio)
        {
            if (points.Length < REQUIRED_COUNT)
                throw ThrowErrors.InvalidCurvePoints(nameof(points), REQUIRED_COUNT);

            Span<Vector3> tmp = stackalloc Vector3[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(tmp, ratio);
        }

        public static Vector3 Evaluate(IList<Vector3> points, float ratio)
        {
            if (points == null)
                throw ThrowErrors.NullParameter(nameof(points));

            if (points.Count < REQUIRED_COUNT)
                throw ThrowErrors.InvalidCurvePoints(nameof(points), REQUIRED_COUNT);

            Span<Vector3> tmp = stackalloc Vector3[points.Count];
            points.CopyTo(tmp);
            return EvaluateInternal(tmp, ratio);
        }

        private static Vector3 EvaluateInternal(Span<Vector3> tmp, float ratio)
        {
            ratio = ratio.Clamp01();
            int counter = tmp.Length - 1;
            int times = counter;

            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < counter; j++)
                {
                    tmp[j] = Vector3.LerpUnclamped(tmp[j], tmp[j + 1], ratio);
                }

                counter--;
            }

            return tmp[0];
        }

        private static Vector3 EvaluateInternal(Vector3[] tmp, float ratio)
        {
            ratio = ratio.Clamp01();

            int counter = tmp.Length - 1;
            int times = counter;

            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < counter; j++)
                {
                    tmp[j] = Vector3.LerpUnclamped(tmp[j], tmp[j + 1], ratio);
                }

                counter--;
            }

            return tmp[0];
        }
    }
}
