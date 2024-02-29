using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.Mathematics
{
    [Serializable]
    public sealed class Bezier3
    {
        [SerializeField]
        private Vector3[] _points;

        private Vector3[] _helpPoints;

        public int Count => _points.Length;
        public ref Vector3 this[int index] => ref _points[index];

        public Bezier3(Vector3[] points)
        {
            if (points.IsNullOrEmpty())
                throw ThrowErrors.InvalidArrayArgument(nameof(points));

            _points = points;
        }

        public Vector3 Evaluate(float ratio)
        {
            if (_helpPoints == null)
                _helpPoints = new Vector3[_points.Length];

            _points.CopyTo(_helpPoints, 0);
            return EvaluateInternal(_helpPoints, ratio);
        }

        public static Vector3 Evaluate(Vector3 orig, Vector3 dest, Vector3 helpPoint, float ratio)
        {
            ratio = ratio.Clamp01();
            Vector3 p1 = Vector3.LerpUnclamped(orig, helpPoint, ratio);
            Vector3 p2 = Vector3.LerpUnclamped(helpPoint, dest, ratio);
            return Vector3.LerpUnclamped(p1, p2, ratio);
        }

        public static Vector3 Evaluate(Span<Vector3> points, float ratio)
        {
            Span<Vector3> tmp = stackalloc Vector3[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(tmp, ratio);
        }

        public static Vector3 Evaluate(Vector3[] points, float ratio)
        {
            Span<Vector3> tmp = stackalloc Vector3[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(tmp, ratio);
        }

        public static Vector3 Evaluate(IList<Vector3> points, float ratio)
        {
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
