using System.IO;
using UnityEngine;

namespace UU.Sound.SoundProviderStuff
{
    public sealed class DefaultClipLoader : ClipLoader
    {
        private readonly string PATH;

        public DefaultClipLoader(string pathToSoundAssets)
        {
            PATH = pathToSoundAssets;
        }

        public AudioClip LoadClip(string name)
        {
            var res = Resources.Load<AudioClip>(PATH + name);

            if (res == null)
            {
                throw new InvalidDataException(string.Concat("There is no any AudioClip asset with the name: ", PATH, name));
            }

            return res;
        }
    }
}
