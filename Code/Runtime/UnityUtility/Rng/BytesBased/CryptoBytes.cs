using System;
using System.Security.Cryptography;

namespace UnityUtility.Rng.BytesBased
{
    internal class CryptoBytes : IRandomBytesProvider
    {
        private RNGCryptoServiceProvider _rng;
        private byte[] _buf;

        public CryptoBytes()
        {
            _rng = new RNGCryptoServiceProvider();
        }

        ~CryptoBytes()
        {
            _rng.Dispose();
        }

        public void GetBytes(byte[] buffer)
        {
            _rng.GetBytes(buffer);
        }

        public void GetBytes(Span<byte> buffer)
        {
#if UNITY_2021_2_OR_NEWER
            _rng.GetBytes(buffer);
#else
            for (int i = 0; i < buffer.Length; i++)
            {
                _rng.GetBytes(_buf);
                buffer[i] = _buf[0];
            }
#endif
        }
    }
}
