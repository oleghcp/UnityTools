using System;
using OlegHcp.Tools;
using UR = UnityEngine.Random;

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
        private static IRng _default;

        public static IRng Default
        {
            get => _default ?? (_default = new BuiltinRngWrapper());
            set
            {
                if (value == null)
                    throw ThrowErrors.NullParameter("value");

                _default = value;
            }
        }

        public abstract void NextBytes(byte[] buffer);
        public abstract void NextBytes(Span<byte> buffer);

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw ThrowErrors.MinMax(nameof(minValue), nameof(maxValue));

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
        private class BuiltinRngWrapper : IRng
        {
            public int Next(int minValue, int maxValue)
            {
                return UR.Range(minValue, maxValue);
            }

            public int Next(int maxValue)
            {
                return UR.Range(0, maxValue);
            }

            public float Next(float minValue, float maxValue)
            {
                return UR.Range(minValue, maxValue);
            }

            public float Next(float maxValue)
            {
                return UR.Range(0f, maxValue);
            }

            public void NextBytes(byte[] buffer)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)UR.Range(0, 256);
                };
            }

            public void NextBytes(Span<byte> buffer)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)UR.Range(0, 256);
                };
            }
        }
        #endregion
    }
}
