using System;

namespace UnityUtility.Rng.BytesBased
{
    internal class TimeBytes : IRandomBytesProvider
    {
        private readonly byte _multiplier;

        private uint _seed;
        private uint _ticks;

        public TimeBytes()
        {
            _seed = (uint)RngHelper.GenerateSeed();
            _multiplier = (byte)(_seed % 8 + 2);
        }

        public void GetBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = RandomByte();
            }
        }

        public void GetBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = RandomByte();
            }
        }

        private byte RandomByte()
        {
            uint newTicks = (uint)Environment.TickCount;
            _ticks = _ticks < newTicks ? newTicks : _ticks + 1;
            _seed = _multiplier * _seed + _ticks;
            return (byte)(_seed % 256);
        }
    }
}
