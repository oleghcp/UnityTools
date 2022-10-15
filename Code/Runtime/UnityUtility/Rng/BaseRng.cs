using System;
using System.Runtime.CompilerServices;
using UnityUtilityTools;

namespace UnityUtility.Rng
{
#if !UNITY_2021_2_OR_NEWER
    [Serializable]
#endif
    public sealed class BaseRng : Random, IRng
    {
        public BaseRng() : base() { }

        public BaseRng(int seed) : base(seed) { }

        public float Next(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            return UnityEngine.Mathf.LerpUnclamped(minValue, maxValue, (float)Sample());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Next(float maxValue)
        {
            return Next(0f, maxValue);
        }

#if !UNITY_2021_2_OR_NEWER
        public void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)base.Next(256);
            }
        }
#endif
    }
}
