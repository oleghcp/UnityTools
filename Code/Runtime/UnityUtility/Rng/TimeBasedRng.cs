using UnityUtility.Rng.BytesBased;

namespace UnityUtility.Rng
{
    public class TimeBasedRng : BytesBasedRng
    {
        public TimeBasedRng() : base(new TimeBytes())
        {

        }
    }
}
