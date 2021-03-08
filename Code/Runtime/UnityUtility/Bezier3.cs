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
        [SerializeField, HideInInspector]
        private Vector3[] _points;

        public Vector3 Origin
        {
            get { return _points[0]; }
            set { _points[0] = value; }
        }

        public Vector3 Dest
        {
            get { return _points.FromEnd(0); }
            set { _points[_points.Length - 1] = value; }
        }

        public Vector3 this[int index]
        {
            get { return _points[index + 1]; }
            set { _points[index + 1] = value; }
        }

        public int Count
        {
            get { return _points.Length - 2; }
        }

        public Bezier3(Vector3 orig, Vector3 dest, int helpPoints)
        {
            if (helpPoints < 1)
                throw Errors.ZeroParameter(nameof(helpPoints));

            _points = new Vector3[helpPoints + 2];
            _points[0] = orig;
            _points[_points.Length - 1] = dest;
        }

        public Bezier3(Vector3 orig, Vector3 dest, Vector3 helpPoint)
        {
            _points = new[] { orig, helpPoint, dest };
        }

        public Bezier3(Vector3 orig, Vector3 dest, Vector3 helpPoint0, Vector3 helpPoint1)
        {
            _points = new[] { orig, helpPoint0, helpPoint1, dest };
        }

        public Bezier3(Vector3 orig, Vector3 dest, Vector3 helpPoint0, Vector3 helpPoint1, Vector3 helpPoint2)
        {
            _points = new[] { orig, helpPoint0, helpPoint1, helpPoint2, dest };
        }

        public Bezier3(Vector3 orig, Vector3 dest, params Vector3[] helpPoints)
        {
            if (helpPoints.IsNullOrEmpty())
                throw Errors.InvalidArrayArgument(nameof(helpPoints));

            _points = new Vector3[helpPoints.Length + 2];
            _points[0] = orig;
            _points.FromEnd(0) = dest;
            helpPoints.CopyTo(_points, 1);
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
    }
}
