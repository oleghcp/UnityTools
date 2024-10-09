using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.Mathematics
{
    [Serializable]
    public class Bezier2
    {
        internal const int REQUIRED_COUNT = 3;

        [SerializeField]
        private Vector2[] _points;

        private Vector2[] _helpPoints;

        public int Count => _points.Length;
        public ref Vector2 this[int index] => ref _points[index + 1];

        public Bezier2(Vector2[] points)
        {
            if (points == null)
                throw ThrowErrors.NullParameter(nameof(points));

            if (points.Length < REQUIRED_COUNT)
                throw ThrowErrors.InvalidBezierPoints(nameof(points));

            _points = points;
        }

        public Vector2 Evaluate(float ratio)
        {
            if (_helpPoints == null)
                _helpPoints = new Vector2[_points.Length];

            _points.CopyTo(_helpPoints, 0);
            return EvaluateInternal(_helpPoints, ratio);
        }

        public static Vector2 Evaluate(in Vector2 origin, in Vector2 destination, in Vector2 controlPoint, float ratio)
        {
            ratio = ratio.Clamp01();
            Vector2 p1 = Vector2.LerpUnclamped(origin, controlPoint, ratio);
            Vector2 p2 = Vector2.LerpUnclamped(controlPoint, destination, ratio);
            return Vector2.LerpUnclamped(p1, p2, ratio);
        }

        public static Vector2 Evaluate(Vector2[] points, float ratio)
        {
#if UNITY_2021_2_OR_NEWER || !UNITY
            return Evaluate((Span<Vector2>)points, ratio);
#else
            return Evaluate((IList<Vector2>)points, ratio);
#endif
        }

        public static Vector2 Evaluate(Span<Vector2> points, float ratio)
        {
            if (points.Length < REQUIRED_COUNT)
                throw ThrowErrors.InvalidBezierPoints(nameof(points));

            Span<Vector2> tmp = stackalloc Vector2[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(tmp, ratio);
        }

        public static Vector2 Evaluate(IList<Vector2> points, float ratio)
        {
            if (points == null)
                throw ThrowErrors.NullParameter(nameof(points));

            if (points.Count < REQUIRED_COUNT)
                throw ThrowErrors.InvalidBezierPoints(nameof(points));

            Span<Vector2> tmp = stackalloc Vector2[points.Count];
            points.CopyTo(tmp);
            return EvaluateInternal(tmp, ratio);
        }

        private static Vector2 EvaluateInternal(Span<Vector2> tmp, float ratio)
        {
            ratio = ratio.Clamp01();
            int counter = tmp.Length - 1;
            int times = counter;

            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < counter; j++)
                {
                    tmp[j] = Vector2.LerpUnclamped(tmp[j], tmp[j + 1], ratio);
                }

                counter--;
            }

            return tmp[0];
        }

        private static Vector2 EvaluateInternal(Vector2[] tmp, float ratio)
        {
            ratio = ratio.Clamp01();
            int counter = tmp.Length - 1;
            int times = counter;

            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < counter; j++)
                {
                    tmp[j] = Vector2.LerpUnclamped(tmp[j], tmp[j + 1], ratio);
                }

                counter--;
            }

            return tmp[0];
        }
    }
}
