using System;
using System.Globalization;
using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.Engine;
using UnityUtility.Tools;

namespace UnityUtility.Mathematics
{
    [Serializable]
    public struct Circle : IEquatable<Circle>, IFormattable
    {
        private static readonly Circle _unit = new Circle(Vector2.zero, 1f);

        public Vector2 Position;
        public float Radius;

        public static Circle Unit => _unit;
        public float Diameter => Radius * 2f;

        public Circle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public Circle(in Vector3 packedSphere)
        {
            Position = packedSphere;
            Radius = packedSphere.z;
        }

        public void Deconstruct(out Vector2 position, out float radius)
        {
            position = Position;
            radius = Radius;
        }

        public bool Contains(Vector2 point)
        {
            return Distance(point, Position) <= Radius;
        }

        public bool Overlaps(in Circle other)
        {
            return Distance(Position, other.Position) <= Radius + other.Radius;
        }

        public float GetArea()
        {
            return MathUtility.GetCircleArea(Radius);
        }

        public float GetCircumference()
        {
            return MathUtility.GetCircumference(Radius);
        }

        public Rect GetBounds()
        {
            return new Rect(Position.x - Radius, Position.y - Radius, Diameter, Diameter);
        }

        public RaycastResult Raycast(in Ray2D ray, out Vector2 hitPoint)
        {
            hitPoint = default;

            if (Contains(ray.origin))
                return RaycastResult.Inside;

            Vector2 vectorToCenter = Position - ray.origin;

            if (Vector2.Dot(vectorToCenter, ray.direction) <= 0f)
                return RaycastResult.None;

            Vector2 rayEndPoint = ray.origin + vectorToCenter.Project(ray.direction);
            float distanceFromCenterToEndPoint = Distance(Position, rayEndPoint);

            if (distanceFromCenterToEndPoint > Radius)
                return RaycastResult.None;

            Ray2D rayFromEndPointToOrigin = new Ray2D(rayEndPoint, ray.origin - rayEndPoint);
            hitPoint = rayFromEndPointToOrigin.GetPoint(MathF.Sqrt(Radius * Radius - distanceFromCenterToEndPoint * distanceFromCenterToEndPoint));
            return RaycastResult.Hit;
        }

        public RaycastResult Raycast(in Ray2D ray, out Vector2 hitPoint, float distance)
        {
            RaycastResult result = Raycast(ray, out hitPoint);

            if (result == RaycastResult.Hit && Distance(ray.origin, hitPoint) > distance)
                result = RaycastResult.None;

            return result;
        }

        private float Distance(in Vector2 a, in Vector2 b)
        {
            var (x, y) = a - b;
            return MathF.Sqrt(x * x + y * y);
        }

        #region Regular Stuff
        public override int GetHashCode()
        {
            return Helper.GetHashCode(Position.GetHashCode(), Radius.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return obj is Circle circle && circle.Equals(this);
        }

        public bool Equals(Circle other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format.IsNullOrEmpty())
                format = "F2";

            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;

#if UNITY_2020_1_OR_NEWER
            string stringPosition = Position.ToString(format, formatProvider);
#else
            string stringPosition = Position.ToString(format);
#endif

            return string.Format(formatProvider, "(Pos:{0}, Rad:{1})", stringPosition, Radius.ToString(format, formatProvider));
        }

        public static bool operator !=(Circle a, Circle b)
        {
            return !(a == b);
        }

        public static bool operator ==(Circle a, Circle b)
        {
            return a.Radius == b.Radius && a.Position == b.Position;
        }
        #endregion
    }
}
