using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility
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

            ratio = ratio.Clamp01();
            int counter = _points.Length - 1;
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

        public static Vector2 Evaluate(Vector2 orig, Vector2 dest, Vector2 helpPoint, float ratio)
        {
            ratio = ratio.Clamp01();
            Vector2 p1 = Vector2.LerpUnclamped(orig, helpPoint, ratio);
            Vector2 p2 = Vector2.LerpUnclamped(helpPoint, dest, ratio);
            return Vector2.LerpUnclamped(p1, p2, ratio);
        }

        public static Vector2 Evaluate(float ratio, Span<Vector2> points)
        {
            Span<Vector2> tmp = stackalloc Vector2[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(ratio, tmp);
        }

        public static Vector2 Evaluate(float ratio, Vector2[] points)
        {
            Span<Vector2> tmp = stackalloc Vector2[points.Length];
            points.CopyTo(tmp);
            return EvaluateInternal(ratio, tmp);
        }

        private static Vector2 EvaluateInternal(float ratio, Span<Vector2> tmp)
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
