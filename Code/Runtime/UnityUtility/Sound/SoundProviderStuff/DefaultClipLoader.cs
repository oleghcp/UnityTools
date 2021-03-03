using System.IO;
using UnityEngine;

namespace UnityUtility.Sound.SoundProviderStuff
{
    public sealed class DefaultClipLoader : IClipLoader
    {
        private readonly string PATH;

        public DefaultClipLoader(string pathToSoundAssets)
        {
            PATH = pathToSoundAssets;
        }

        public AudioClip LoadClip(string name)
        {
            AudioClip res = Resources.Load<AudioClip>(PATH + name);

            if (res == null)
                throw new InvalidDataException($"There is no any AudioClip asset with the name: {PATH}{name}");

            return res;
        }
    }
}
