using UnityUtility.Rng.BytesBased;

namespace UnityUtility.Rng
{
    public class GuidRng : BytesBasedRng
    {
        public GuidRng() : base(new GuidBytes())
        {
        }
    }
}
