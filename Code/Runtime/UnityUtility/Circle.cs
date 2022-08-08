using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtilityTools;

namespace UnityUtility
{
    [Serializable]
    public struct Circle : IEquatable<Circle>
    {
        public Vector2 Position;
        public float Radius;

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
            return Vector2.Distance(point, Position) < Radius;
        }

        public bool Overlaps(in Circle other)
        {
            return Vector2.Distance(Position, other.Position) < Radius + other.Radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArea()
        {
            return MathUtility.GetCircleArea(Radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetLength()
        {
            return MathUtility.GetCircumference(Radius);
        }

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

        public static bool operator !=(Circle a, Circle b)
        {
            return !(a == b);
        }

        public static bool operator ==(Circle a, Circle b)
        {
            return a.Radius == b.Radius && a.Position == b.Position;
        }
    }
}
