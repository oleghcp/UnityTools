using UnityEngine;

namespace UU.Sound.SoundProviderStuff
{
    public abstract class SoundObjectInfo : Script
    {
        internal abstract string ClipName { get; }
        internal abstract AudioSource AudioSource { get; }
        internal abstract void Stop();
    }
}
