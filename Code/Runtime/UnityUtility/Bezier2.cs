using System;
using System.Collections.Generic;
using UnityEngine;

namespace UU
{
    public class Bezier2
    {
        private Vector2[] m_points;
        private Vector2[] m_tmp;

        public Vector2 Origin
        {
            get { return m_points[0]; }
            set { m_points[0] = value; }
        }

        public Vector2 Dest
        {
            get { return m_points.GetLast(); }
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
                throw new ArgumentOutOfRangeException(nameof(helpPoints), "The parameter must be more than zero.");

            m_points = new Vector2[helpPoints + 2];
            m_points[0] = orig;
            m_points[m_points.Length - 1] = dest;
            m_tmp = new Vector2[m_points.Length];
        }

        public Bezier2(Vector2 orig, Vector2 dest, Vector2 helpPoint)
        {
            m_points = new[] { orig, helpPoint, dest };
            m_tmp = new Vector2[m_points.Length];
        }

        public Bezier2(Vector2 orig, Vector2 dest, Vector2 helpPoint0, Vector2 helpPoint1)
        {
            m_points = new[] { orig, helpPoint0, helpPoint1, dest };
            m_tmp = new Vector2[m_points.Length];
        }

        public Bezier2(Vector2 orig, Vector2 dest, Vector2 helpPoint0, Vector2 helpPoint1, Vector2 helpPoint2)
        {
            m_points = new[] { orig, helpPoint0, helpPoint1, helpPoint2, dest };
            m_tmp = new Vector2[m_points.Length];
        }

        public Bezier2(Vector2 orig, Vector2 dest, params Vector2[] helpPoints)
        {
            m_points = new Vector2[helpPoints.Length + 2];
            m_points[0] = orig;
            m_points.GetLast() = dest;
            helpPoints.CopyTo(m_points, 1);
            m_tmp = new Vector2[m_points.Length];
        }

        public Vector2 Evaluate(float ratio)
        {
            Array.Copy(m_points, m_tmp, m_points.Length);

            int counter = m_points.Length - 1;
            int times = counter;

            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < counter; j++)
                {
                    m_tmp[j] = Vector2.Lerp(m_tmp[j], m_tmp[j + 1], ratio);
                }

                counter--;
            }

            return m_tmp[0];
        }

        public static Vector2 Evaluate(Vector2 orig, Vector2 dest, Vector2 helpPoint, float ratio)
        {
            Vector2 p1 = Vector2.Lerp(orig, helpPoint, ratio);
            Vector2 p2 = Vector2.Lerp(helpPoint, dest, ratio);
            return Vector2.Lerp(p1, p2, ratio);
        }
    }
}
