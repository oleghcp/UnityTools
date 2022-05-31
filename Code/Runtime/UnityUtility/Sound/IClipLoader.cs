using UnityEngine;

#if INCLUDE_AUDIO
namespace UnityUtility.Sound
{
    public interface IClipLoader
    {
        AudioClip LoadClip(string name);
    }
}
#endif
