using System;
using UnityUtility.Rng.BytesBased;

namespace UnityUtility.Rng
{
    [Serializable]
    public class Xorshift64Rng : BytesBasedRng
    {
        public Xorshift64Rng() : base(new Xorshift64Bytes())
        {
        }

        public Xorshift64Rng(int seed) : base(new Xorshift64Bytes(seed))
        {
        }

        public Xorshift64Rng(int seed, int a, int b, int c) : base(new Xorshift64Bytes(seed, a, b, c))
        {
        }
    }
}
