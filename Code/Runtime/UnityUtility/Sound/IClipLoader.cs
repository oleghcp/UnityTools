using UnityEngine;

namespace UnityUtility.Sound
{
    public interface IClipLoader
    {
        AudioClip LoadClip(string name);
    }
}
