using UnityEngine;

namespace UU.Sound
{
    public interface ClipLoader
    {
        AudioClip LoadClip(string name);
    }
}
