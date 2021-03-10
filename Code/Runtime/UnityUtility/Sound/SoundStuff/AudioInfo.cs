using UnityEngine;

namespace UnityUtility.Sound.SoundStuff
{
    public abstract class AudioInfo : MonoBehaviour
    {
        internal abstract string ClipName { get; }
        internal abstract AudioSource AudioSource { get; }
        internal abstract void Stop();
    }
}
