#if INCLUDE_AUDIO
using UnityEngine;

namespace UnityUtility.Sound.SoundStuff
{
    [DisallowMultipleComponent]
    public abstract class AudioInfo : MonoBehaviour
    {
        internal abstract string ClipName { get; }
        internal abstract AudioSource AudioSource { get; }
        internal abstract void Stop();
    }
}
#endif
