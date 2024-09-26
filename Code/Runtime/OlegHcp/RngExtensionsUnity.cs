using System;
using OlegHcp.Engine;
using OlegHcp.Mathematics;
using UnityEngine;

namespace OlegHcp
{
    public static class RngExtensionsUnity
    {
        /// <summary>
        /// Returns a random point on the circle line with radius 1.
        /// </summary>
        public static Vector2 GetOnUnitCircle(this IRng self)
        {
            float angle = self.Next(MathF.PI * 2f);
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        /// <summary>
        /// Returns a random point inside a circle with radius 1.
        /// </summary>
        public static Vector2 GetInsideUnitCircle(this IRng self)
        {
            Rect bounds = Circle.Unit.GetBounds();

            Vector2 vector;
            do
            {
                vector = self.GetInBounds(bounds);
            } while (vector.magnitude > 1f);

            return vector;
        }

        /// <summary>
        /// Returns a random point on the surface of a sphere with radius 1.
        /// </summary>
        public static Vector3 GetOnUnitSphere(this IRng self)
        {
            Bounds bounds = Sphere.Unit.GetBounds();
            Vector3 vector;
            float magnitude;

            do
            {
                vector = self.GetInBounds(bounds);
                magnitude = vector.magnitude;

            } while (magnitude <= MathUtility.kEpsilon || magnitude > 1f);

            return vector / magnitude;
        }

        /// <summary>
        /// Returns a random point inside a sphere with radius 1.
        /// </summary>
        public static Vector3 GetInsideUnitSphere(this IRng self)
        {
            Bounds bounds = Sphere.Unit.GetBounds();

            Vector3 vector;
            do
            {
                vector = self.GetInBounds(bounds);
            } while (vector.magnitude > 1f);

            return vector;
        }

        /// <summary>
        /// Returns a random rotation.
        /// </summary>
        public static Quaternion GetRandomRotation(this IRng self)
        {
            Vector3 up = self.GetOnUnitCircle();
            Vector3 forward = self.GetOnUnitSphere();

            Vector3 axis = Vector3.Cross(Vector3.forward, forward).normalized;

            if (axis.magnitude > MathUtility.kEpsilon)
            {
                float angle = Vector3.Angle(Vector3.forward, forward);
                up = up.GetRotated(axis, angle);
            }

            return Quaternion.LookRotation(forward, up);
        }

        /// <summary>
        /// Returns a random color32 with the specified alfa.
        /// </summary>
        public static Color GetRandomColor(this IRng self)
        {
            return new Color
            {
                r = self.Next(1f),
                g = self.Next(1f),
                b = self.Next(1f),
                a = 1f,
            };
        }

        public static Vector2 GetInBounds(this IRng self, in Rect bounds)
        {
            return new Vector2(self.Next(bounds.xMin, bounds.xMax),
                               self.Next(bounds.yMin, bounds.yMax));
        }

        public static Vector3 GetInBounds(this IRng self, in Bounds bounds)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            
            return new Vector3()
            {
                x = self.Next(min.x, max.x),
                y = self.Next(min.y, max.y),
                z = self.Next(min.z, max.z),
            };
        }
    }
}
