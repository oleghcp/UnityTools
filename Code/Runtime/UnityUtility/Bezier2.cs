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
        [SerializeField, HideInInspector]
        private Vector2[] _points;

        public Vector2 Origin
        {
            get => _points[0];
            set => _points[0] = value;
        }

        public Vector2 Dest
        {
            get => _points.FromEnd(0);
            set => _points[_points.Length - 1] = value;
        }

        public Vector2 this[int index]
        {
            get => _points[index + 1];
            set => _points[index + 1] = value;
        }

        public int Count => _points.Length - 2;

        public Bezier2(Vector2 orig, Vector2 dest, int helpPoints)
        {
            if (helpPoints < 1)
                throw Errors.ZeroParameter(nameof(helpPoints));

            _points = new Vector2[helpPoints + 2];
            _points[0] = orig;
            _points[_points.Length - 1] = dest;
        }

        public Bezier2(Vector2 orig, Vector2 dest, Vector2 helpPoint)
        {
            _points = new[] { orig, helpPoint, dest };
        }

        public Bezier2(Vector2 orig, Vector2 dest, Vector2 helpPoint0, Vector2 helpPoint1)
        {
            _points = new[] { orig, helpPoint0, helpPoint1, dest };
        }

        public Bezier2(Vector2 orig, Vector2 dest, Vector2 helpPoint0, Vector2 helpPoint1, Vector2 helpPoint2)
        {
            _points = new[] { orig, helpPoint0, helpPoint1, helpPoint2, dest };
        }

        public Bezier2(Vector2 orig, Vector2 dest, params Vector2[] helpPoints)
        {
            if (helpPoints.IsNullOrEmpty())
                throw Errors.InvalidArrayArgument(nameof(helpPoints));

            _points = new Vector2[helpPoints.Length + 2];
            _points[0] = orig;
            _points.FromEnd(0) = dest;
            helpPoints.CopyTo(_points, 1);
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
    }
}
