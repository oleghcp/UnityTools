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
#if UNITY_2018_3_OR_NEWER
        void NextBytes(Span<byte> buffer);
#endif
    }
}
