using UnityEngine;

namespace UnityUtility.Sound
{
    public interface ClipLoader
    {
        AudioClip LoadClip(string name);
    }
}
