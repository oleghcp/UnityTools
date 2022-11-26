using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtilityTools;

namespace UnityUtility.Mathematics
{
    [Serializable]
    public struct Circle : IEquatable<Circle>, IFormattable
    {
        public Vector2 Position;
        public float Radius;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArea()
        {
            return MathUtility.GetCircleArea(Radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetCircumference()
        {
            return MathUtility.GetCircumference(Radius);
        }

        public Rect GetBounds()
        {
            return new Rect(Position.x - Radius, Position.y - Radius, Diameter, Diameter);
        }

        //public bool Raycast(Ray2D ray, out Vector2 point)
        //{
        //    if (Contains(ray.origin))
        //    {
        //        point = default;
        //        return false;
        //    }

        //    throw new NotImplementedException();
        //}

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
