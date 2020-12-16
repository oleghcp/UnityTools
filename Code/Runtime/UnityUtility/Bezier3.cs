using System;
using System.Collections.Generic;
using UnityUtilityTools;
using UnityEngine;

namespace UnityUtility
{
    public sealed class Bezier3
    {
        private Vector3[] m_points;
        private Vector3[] m_tmp;

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
            m_tmp = new Vector3[m_points.Length];
        }

        public Bezier3(Vector3 orig, Vector3 dest, Vector3 helpPoint)
        {
            m_points = new[] { orig, helpPoint, dest };
            m_tmp = new Vector3[m_points.Length];
        }

        public Bezier3(Vector3 orig, Vector3 dest, Vector3 helpPoint0, Vector3 helpPoint1)
        {
            m_points = new[] { orig, helpPoint0, helpPoint1, dest };
            m_tmp = new Vector3[m_points.Length];
        }

        public Bezier3(Vector3 orig, Vector3 dest, Vector3 helpPoint0, Vector3 helpPoint1, Vector3 helpPoint2)
        {
            m_points = new[] { orig, helpPoint0, helpPoint1, helpPoint2, dest };
            m_tmp = new Vector3[m_points.Length];
        }

        public Bezier3(Vector3 orig, Vector3 dest, params Vector3[] helpPoints)
        {
            m_points = new Vector3[helpPoints.Length + 2];
            m_points[0] = orig;
            m_points.FromEnd(0) = dest;
            helpPoints.CopyTo(m_points, 1);
            m_tmp = new Vector3[m_points.Length];
        }

        public Vector3 Evaluate(float ratio)
        {
            Array.Copy(m_points, m_tmp, m_points.Length);

            int counter = m_points.Length - 1;
            int times = counter;

            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < counter; j++)
                {
                    m_tmp[j] = Vector3.Lerp(m_tmp[j], m_tmp[j + 1], ratio);
                }

                counter--;
            }

            return m_tmp[0];
        }

        public static Vector3 Evaluate(Vector3 orig, Vector3 dest, Vector3 helpPoint, float ratio)
        {
            Vector3 p1 = Vector3.Lerp(orig, helpPoint, ratio);
            Vector3 p2 = Vector3.Lerp(helpPoint, dest, ratio);
            return Vector3.Lerp(p1, p2, ratio);
        }
    }
}
