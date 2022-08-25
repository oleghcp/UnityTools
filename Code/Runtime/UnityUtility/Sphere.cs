using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtilityTools;

namespace UnityUtility
{
    [Serializable]
    public struct Sphere : IEquatable<Sphere>, IFormattable
    {
        public Vector3 Position;
        public float Radius;

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
            return Vector3.Distance(point, Position) <= Radius;
        }

        public bool Overlaps(in Sphere other)
        {
            return Vector3.Distance(Position, other.Position) <= Radius + other.Radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetVolume()
        {
            return MathUtility.GetSphereVolume(Radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetSurface()
        {
            return MathUtility.GetSphereSurface(Radius);
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(Position.GetHashCode(), Radius.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return obj is Sphere sphere && sphere.Equals(this);
        }

        public bool Equals(Sphere other)
        {
            return this == other;
        }

        public static bool operator !=(Sphere a, Sphere b)
        {
            return !(a == b);
        }

        public static bool operator ==(Sphere a, Sphere b)
        {
            return a.Radius == b.Radius && a.Position == b.Position;
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

            return string.Format(formatProvider, "(Pos:{0}, Rad:{1})", Position.ToString(format, formatProvider), Radius.ToString(format, formatProvider));
        }
    }
}
