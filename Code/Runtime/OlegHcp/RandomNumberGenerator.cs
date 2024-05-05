using System;
using OlegHcp.Tools;

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

    [Serializable]
    public abstract class RandomNumberGenerator : IRng
    {
        private static IRng _default = new BuiltIn();

        public static IRng Default => _default;

        public abstract void NextBytes(byte[] buffer);
        public abstract void NextBytes(Span<byte> buffer);

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw ThrowErrors.MinMax(nameof(minValue), nameof(maxValue));

            if (minValue == maxValue)
                return minValue;

            return NextInternal(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw ThrowErrors.NegativeParameter(nameof(maxValue));

            if (maxValue == 0)
                return 0;

            return NextInternal(0, maxValue);
        }

        public float Next(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw ThrowErrors.MinMax(nameof(minValue), nameof(maxValue));

            return NextInternal(minValue, maxValue);
        }

        public float Next(float maxValue)
        {
            if (maxValue < 0f)
                throw ThrowErrors.NegativeParameter(nameof(maxValue));

            return NextInternal(0f, maxValue);
        }

        protected abstract int NextInternal(int minValue, int maxValue);
        protected abstract float NextInternal(float minValue, float maxValue);

        #region Builtin
        private class BuiltIn : IRng
        {
            public int Next(int minValue, int maxValue)
            {
                return UnityEngine.Random.Range(minValue, maxValue);
            }

            public int Next(int maxValue)
            {
                return UnityEngine.Random.Range(0, maxValue);
            }

            public float Next(float minValue, float maxValue)
            {
                return UnityEngine.Random.Range(minValue, maxValue);
            }

            public float Next(float maxValue)
            {
                return UnityEngine.Random.Range(0f, maxValue);
            }

            public void NextBytes(byte[] buffer)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)UnityEngine.Random.Range(0, 256);
                };
            }

            public void NextBytes(Span<byte> buffer)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)UnityEngine.Random.Range(0, 256);
                };
            }
        }
        #endregion
    }
}
