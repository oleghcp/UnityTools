using System;

namespace UnityUtility.Rng.BytesBased
{
    internal class TimeBytes : IRandomBytesProvider
    {
        private uint _ticks;
        private uint _num;

        public TimeBytes()
        {
            uint seed = (uint)Environment.TickCount;
            if (seed < byte.MaxValue)
                seed = byte.MaxValue;
            _num = seed;
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
            _ticks = _ticks != newTicks ? newTicks : newTicks + 1;
            _num += _ticks * 3;
            return (byte)(_num % 256);
        }
    }
}
