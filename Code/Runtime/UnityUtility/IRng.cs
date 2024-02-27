﻿using System;

namespace OlegHcp
{
    public interface IRng
    {
        int Next(int minValue, int maxValue);
        int Next(int maxValue);
        float Next(float minValue, float maxValue);
        float Next(float maxValue);
        void NextBytes(byte[] buffer);
        void NextBytes(Span<byte> buffer);
    }
}
