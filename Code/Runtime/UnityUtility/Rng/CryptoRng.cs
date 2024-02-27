using OlegHcp.Rng.BytesBased;

namespace OlegHcp.Rng
{
    public class CryptoRng : BytesBasedRng
    {
        public CryptoRng() : base(new CryptoBytes())
        {

        }
    }
}
