#if INCLUDE_AUDIO
using UnityEngine;

namespace OlegHcp.Sound
{
    public interface IClipLoader
    {
        AudioClip LoadClip(string name);
    }
}
#endif
