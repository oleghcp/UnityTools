using UnityEngine;

#if !UNITY_2019_1_OR_NEWER || INCLUDE_AUDIO
namespace UnityUtility.Sound
{
    public interface IClipLoader
    {
        AudioClip LoadClip(string name);
    }
}
#endif
