using UnityUtility.Rng.BytesBased;

namespace UnityUtility.Rng
{
    public class CryptoBasedRng : BytesBasedRng
    {
        public CryptoBasedRng() : base(new CryptoBytes())
        {

        }
    }
}
