using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility
{
    public sealed class Bezier3
    {
        private Vector3[] m_points;

        public Vector3 Origin
        {
            get { return m_points[0]; }
            set { m_points[0] = value; }
        }

        public Vector3 Dest
        {
            get { return m_points.FromEnd(0); }
            set { m_points[m_points.Length - 1] = value; }
        }

        public Vector3 this[int index]
        {
            get { return m_points[index + 1]; }
            set { m_points[index + 1] = value; }
        }

        public int Count
        {
            get { return m_points.Length - 2; }
        }

        public Bezier3(Vector3 orig, Vector3 dest, int helpPoints)
        {
            if (helpPoints < 1)
                throw Errors.ZeroParameter(nameof(helpPoints));

            m_points = new Vector3[helpPoints + 2];
            m_points[0] = orig;
            m_points[m_points.Length - 1] = dest;
        }

        public Bezier3(Vector3 orig, Vector3 dest, Vector3 helpPoint)
        {
            m_points = new[] { orig, helpPoint, dest };
        }

        public Bezier3(Vector3 orig, Vector3 dest, Vector3 helpPoint0, Vector3 helpPoint1)
        {
            m_points = new[] { orig, helpPoint0, helpPoint1, dest };
        }

        public Bezier3(Vector3 orig, Vector3 dest, Vector3 helpPoint0, Vector3 helpPoint1, Vector3 helpPoint2)
        {
            m_points = new[] { orig, helpPoint0, helpPoint1, helpPoint2, dest };
        }

        public Bezier3(Vector3 orig, Vector3 dest, params Vector3[] helpPoints)
        {
            if (helpPoints.IsNullOrEmpty())
                throw Errors.InvalidArrayArgument(nameof(helpPoints));

            m_points = new Vector3[helpPoints.Length + 2];
            m_points[0] = orig;
            m_points.FromEnd(0) = dest;
            helpPoints.CopyTo(m_points, 1);
        }

        public Vector3 Evaluate(float ratio)
        {
            Span<Vector3> tmp = stackalloc Vector3[m_points.Length];
            m_points.CopyTo(tmp);

            ratio = ratio.Clamp01();
            int counter = m_points.Length - 1;
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
