using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility
{
    [Serializable]
    public sealed class Bezier3
    {
        [SerializeField]
        private Vector3[] _points;

        public int Count => _points.Length;
        public ref Vector3 this[int index] => ref _points[index];

        public Bezier3(Vector3[] points)
        {
            if (points.IsNullOrEmpty())
                throw Errors.InvalidArrayArgument(nameof(points));

            _points = points;
        }

        public Vector3 Evaluate(float ratio)
        {
            Span<Vector3> tmp = stackalloc Vector3[_points.Length];
            _points.CopyTo(tmp);

            ratio = ratio.Clamp01();
            int counter = _points.Length - 1;
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

        public static Vector3 Evaluate(Vector3 orig, Vector3 dest, Vector3 helpPoint, float ratio)
        {
            ratio = ratio.Clamp01();
            Vector3 p1 = Vector3.LerpUnclamped(orig, helpPoint, ratio);
            Vector3 p2 = Vector3.LerpUnclamped(helpPoint, dest, ratio);
            return Vector3.LerpUnclamped(p1, p2, ratio);
        }

        public static Vector3 Evaluate(float ratio, Span<Vector3> points)
        {
            Span<Vector3> tmp = stackalloc Vector3[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(ratio, tmp);
        }

        public static Vector3 Evaluate(float ratio, Vector3[] points)
        {
            Span<Vector3> tmp = stackalloc Vector3[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(ratio, tmp);
        }

        private static Vector3 EvaluateInternal(float ratio, Span<Vector3> tmp)
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
