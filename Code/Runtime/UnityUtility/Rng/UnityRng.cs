using System;
using System.Runtime.CompilerServices;
using Tools;
using UnityEngine;
using Uerng = UnityEngine.Random;

namespace UnityUtility.Rng
{
    public sealed class UnityRng : IRng
    {
        private Uerng.State m_state;

        public UnityRng() : this(Environment.TickCount) { }

        public UnityRng(int seed)
        {
            Uerng.InitState(seed);
            m_state = Uerng.state;
        }

        public int Next(int minValue, int maxValue)
        {
            Uerng.state = m_state;
            int value = Uerng.Range(minValue, maxValue);
            m_state = Uerng.state;
            return value;
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw Errors.NegativeParameter(nameof(maxValue));

            return Next(0, maxValue);
        }

        public float Next(float minValue, float maxValue)
        {
            Uerng.state = m_state;
            float value = Uerng.Range(minValue, maxValue);
            m_state = Uerng.state;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Next(float maxValue)
        {
            return Next(0f, maxValue);
        }

        public double NextDouble()
        {
            Uerng.state = m_state;
            double value = Uerng.Range(0, int.MaxValue);
            m_state = Uerng.state;
            return value / int.MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte NextByte()
        {
            return (byte)Next(0, 256);
        }

        public void NextBytes(byte[] buffer)
        {
            Uerng.state = m_state;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)Uerng.Range(0, 256);
            }
            m_state = Uerng.state;
        }

        public unsafe void NextBytes(byte* arrayPtr, int length)
        {
            if (arrayPtr == null)
                throw new ArgumentNullException(nameof(arrayPtr));

            Uerng.state = m_state;
            for (int i = 0; i < length; i++)
            {
                arrayPtr[i] = (byte)Uerng.Range(0, 256);
            }
            m_state = Uerng.state;
        }

        // -- //

        public static Vector2 GetInsideUnitCircle()
        {
            var prevState = Uerng.state;
            Vector2 result = Uerng.insideUnitCircle;
            Uerng.state = prevState;
            return result;
        }

        public static Vector3 GetInsideUnitSphere()
        {
            var prevState = Uerng.state;
            Vector3 result = Uerng.insideUnitSphere;
            Uerng.state = prevState;
            return result;
        }

        public static Vector3 GetOnUnitSphere()
        {
            var prevState = Uerng.state;
            Vector3 result = Uerng.onUnitSphere;
            Uerng.state = prevState;
            return result;
        }

        public static Quaternion GetRandomRot(bool uniformDistribution)
        {
            var prevState = Uerng.state;
            Quaternion result = uniformDistribution ? Uerng.rotationUniform : Uerng.rotation;
            Uerng.state = prevState;
            return result;
        }        
    }
}
