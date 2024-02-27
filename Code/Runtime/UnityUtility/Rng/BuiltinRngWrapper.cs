using System;
using static UnityEngine.Random;

namespace OlegHcp.Rng
{
    internal class BuiltinRngWrapper : IRng
    {
        public int Next(int minValue, int maxValue)
        {
            return Range(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            return Range(0, maxValue);
        }

        public float Next(float minValue, float maxValue)
        {
            return Range(minValue, maxValue);
        }

        public float Next(float maxValue)
        {
            return Range(0f, maxValue);
        }

        public void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)Range(0, 256);
            };
        }

        public void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)Range(0, 256);
            };
        }
    }
}
