using UnityUtility.Rng.BytesBased;

namespace UnityUtility.Rng
{
    public class CryptoRng : BytesBasedRng
    {
        public CryptoRng() : base(new CryptoBytes())
        {

        }
    }
}
