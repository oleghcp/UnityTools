using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility
{
    public sealed class Bezier2
    {
        private Vector2[] m_points;

        public Vector2 Origin
        {
            get { return m_points[0]; }
            set { m_points[0] = value; }
        }

        public Vector2 Dest
        {
            get { return m_points.FromEnd(0); }
            set { m_points[m_points.Length - 1] = value; }
        }

        public Vector2 this[int index]
        {
            get { return m_points[index + 1]; }
            set { m_points[index + 1] = value; }
        }

        public int Count
        {
            get { return m_points.Length - 2; }
        }

        public Bezier2(Vector2 orig, Vector2 dest, int helpPoints)
        {
            if (helpPoints < 1)
                throw Errors.ZeroParameter(nameof(helpPoints));

            m_points = new Vector2[helpPoints + 2];
            m_points[0] = orig;
            m_points[m_points.Length - 1] = dest;
        }

        public Bezier2(Vector2 orig, Vector2 dest, Vector2 helpPoint)
        {
            m_points = new[] { orig, helpPoint, dest };
        }

        public Bezier2(Vector2 orig, Vector2 dest, Vector2 helpPoint0, Vector2 helpPoint1)
        {
            m_points = new[] { orig, helpPoint0, helpPoint1, dest };
        }

        public Bezier2(Vector2 orig, Vector2 dest, Vector2 helpPoint0, Vector2 helpPoint1, Vector2 helpPoint2)
        {
            m_points = new[] { orig, helpPoint0, helpPoint1, helpPoint2, dest };
        }

        public Bezier2(Vector2 orig, Vector2 dest, params Vector2[] helpPoints)
        {
            if (helpPoints.IsNullOrEmpty())
                throw Errors.InvalidArrayArgument(nameof(helpPoints));

            m_points = new Vector2[helpPoints.Length + 2];
            m_points[0] = orig;
            m_points.FromEnd(0) = dest;
            helpPoints.CopyTo(m_points, 1);
        }

        public Vector2 Evaluate(float ratio)
        {
            Span<Vector2> tmp = stackalloc Vector2[m_points.Length];
            m_points.CopyTo(tmp);

            ratio = ratio.Clamp01();
            int counter = m_points.Length - 1;
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
