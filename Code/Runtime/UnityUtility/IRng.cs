using System;

namespace UnityUtility
{
    public interface IRng
    {
        int Next(int minValue, int maxValue);
        int Next(int maxValue);
        float Next(float minValue, float maxValue);
        float Next(float maxValue);
        double NextDouble();
        byte NextByte();
        void NextBytes(byte[] buffer);
        void NextBytes(Span<byte> buffer);
    }
}
