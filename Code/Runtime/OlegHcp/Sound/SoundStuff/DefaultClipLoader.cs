﻿#if INCLUDE_AUDIO
using System.IO;
using UnityEngine;

namespace OlegHcp.Sound.SoundStuff
{
    public sealed class DefaultClipLoader : IClipLoader
    {
        private readonly string _path;

        public DefaultClipLoader(string pathToSoundAssets)
        {
            _path = pathToSoundAssets;
        }

        public AudioClip LoadClip(string name)
        {
            AudioClip res = Resources.Load<AudioClip>(_path + name);

            if (res == null)
                throw new InvalidDataException($"There is no any AudioClip asset with the name: {_path}{name}");

            return res;
        }
    }
}
#endif
