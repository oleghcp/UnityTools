using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityUtility
{
    [Serializable]
    public struct Sphere
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
            return Vector3.Distance(point, Position) < Radius;
        }

        public bool Overlaps(in Sphere other)
        {
            return Vector3.Distance(Position, other.Position) < Radius + other.Radius;
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

        public static implicit operator Sphere(BoundingSphere sphere)
        {
            return new Sphere(sphere);
        }

        public static implicit operator BoundingSphere(Sphere sphere)
        {
            return new BoundingSphere(sphere.Position, sphere.Radius);
        }
    }
}
