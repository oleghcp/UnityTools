using UnityEngine;

namespace OlegHcp.Engine
{
    public static class QuaternionExtensions
    {
        public static Vector3 Up(this in Quaternion value)
        {
            return value * Vector3.up;
        }

        public static Vector3 Down(this in Quaternion value)
        {
            return value * Vector3.down;
        }

        public static Vector3 Right(this in Quaternion value)
        {
            return value * Vector3.right;
        }

        public static Vector3 Left(this in Quaternion value)
        {
            return value * Vector3.left;
        }

        public static Vector3 Forward(this in Quaternion value)
        {
            return value * Vector3.forward;
        }

        public static Vector3 Back(this in Quaternion value)
        {
            return value * Vector3.back;
        }

        public static void Deconstruct(this in Quaternion value, out float x, out float y, out float z, out float w)
        {
            x = value.x;
            y = value.y;
            z = value.z;
            w = value.w;
        }
    }
}
