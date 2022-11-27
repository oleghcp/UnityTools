using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.CSharp.Collections;
using UnityUtilityTools;

namespace UnityUtility.Mathematics
{
    [Serializable]
    public sealed class Bezier2
    {
        [SerializeField]
        private Vector2[] _points;

        public int Count => _points.Length;
        public ref Vector2 this[int index] => ref _points[index + 1];

        public Bezier2(Vector2[] points)
        {
            if (points.IsNullOrEmpty())
                throw Errors.InvalidArrayArgument(nameof(points));

            _points = points;
        }

        public Vector2 Evaluate(float ratio)
        {
            Span<Vector2> tmp = stackalloc Vector2[_points.Length];
            _points.CopyTo(tmp);
            return EvaluateInternal(tmp, ratio);
        }

        public static Vector2 Evaluate(Vector2 orig, Vector2 dest, Vector2 helpPoint, float ratio)
        {
            ratio = ratio.Clamp01();
            Vector2 p1 = Vector2.LerpUnclamped(orig, helpPoint, ratio);
            Vector2 p2 = Vector2.LerpUnclamped(helpPoint, dest, ratio);
            return Vector2.LerpUnclamped(p1, p2, ratio);
        }

        public static Vector2 Evaluate(Span<Vector2> points, float ratio)
        {
            Span<Vector2> tmp = stackalloc Vector2[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(tmp, ratio);
        }

        public static Vector2 Evaluate(Vector2[] points, float ratio)
        {
            Span<Vector2> tmp = stackalloc Vector2[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(tmp, ratio);
        }

        public static Vector2 Evaluate(IList<Vector2> points, float ratio)
        {
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
    }
}
