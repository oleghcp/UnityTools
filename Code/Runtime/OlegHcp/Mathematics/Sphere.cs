using System;
using System.Globalization;
using UnityEngine;
using OlegHcp.CSharp;
using OlegHcp.Engine;
using OlegHcp.Tools;

namespace OlegHcp.Mathematics
{
    [Serializable]
    public struct Sphere : IEquatable<Sphere>, IFormattable
    {
        private static readonly Sphere _unit = new Sphere(Vector3.zero, 1f);

        public Vector3 Position;
        public float Radius;

        public static Sphere Unit => _unit;
        public float Diameter => Radius * 2f;

        public Sphere(in Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public Sphere(in Vector4 packedSphere)
        {
            Position = new Vector3(packedSphere.x, packedSphere.y, packedSphere.z);
            Radius = packedSphere.w;
        }

        public Sphere(in BoundingSphere sphere)
        {
            Position = sphere.position;
            Radius = sphere.radius;
        }

        public void Deconstruct(out Vector3 position, out float radius)
        {
            position = Position;
            radius = Radius;
        }

        public bool Contains(in Vector3 point)
        {
            return Distance(point, Position) <= Radius;
        }

        public bool Overlaps(in Sphere other)
        {
            return Distance(Position, other.Position) <= Radius + other.Radius;
        }

        public float GetVolume()
        {
            return MathUtility.GetSphereVolume(Radius);
        }

        public float GetSurface()
        {
            return MathUtility.GetSphereSurface(Radius);
        }

        public Bounds GetBounds()
        {
            return new Bounds(Position, Vector3.one * Diameter);
        }

        public RaycastResult Raycast(in Ray ray, out Vector3 hitPoint)
        {
            hitPoint = default;

            if (Contains(ray.origin))
                return RaycastResult.Inside;

            Vector3 vectorToCenter = Position - ray.origin;

            if (Vector3.Dot(vectorToCenter, ray.direction) <= 0f)
                return RaycastResult.None;

            Vector3 rayEndPoint = ray.origin + vectorToCenter.Project(ray.direction);
            float distanceFromCenterToEndPoint = Distance(Position, rayEndPoint);

            if (distanceFromCenterToEndPoint > Radius)
                return RaycastResult.None;

            Ray rayFromEndPointToOrigin = new Ray(rayEndPoint, ray.origin - rayEndPoint);
            hitPoint = rayFromEndPointToOrigin.GetPoint(MathF.Sqrt(Radius * Radius - distanceFromCenterToEndPoint * distanceFromCenterToEndPoint));
            return RaycastResult.Hit;
        }

        public RaycastResult Raycast(in Ray ray, out Vector3 hitPoint, float distance)
        {
            RaycastResult result = Raycast(ray, out hitPoint);

            if (result == RaycastResult.Hit && Distance(ray.origin, hitPoint) > distance)
                result = RaycastResult.None;

            return result;
        }

        private float Distance(in Vector3 a, in Vector3 b)
        {
            var (x, y, z) = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return MathF.Sqrt(x * x + y * y + z * z);
        }

        #region Regular Stuff
        public override int GetHashCode()
        {
            return Helper.GetHashCode(Position.GetHashCode(), Radius.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return obj is Sphere sphere && Equals(sphere);
        }

        public bool Equals(Sphere other)
        {
            return other.Radius == Radius && other.Position == Position;
        }

        public static implicit operator Sphere(BoundingSphere sphere)
        {
            return new Sphere(sphere);
        }

        public static implicit operator BoundingSphere(Sphere sphere)
        {
            return new BoundingSphere(sphere.Position, sphere.Radius);
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

        public static bool operator ==(Sphere a, Sphere b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Sphere a, Sphere b)
        {
            return !a.Equals(b);
        }
        #endregion
    }
}
