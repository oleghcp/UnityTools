using OlegHcp.Rng.BytesBased;

namespace OlegHcp.Rng
{
    public class GuidRng : BytesBasedRng
    {
        public GuidRng() : base(new GuidBytes())
        {
        }
    }
}
