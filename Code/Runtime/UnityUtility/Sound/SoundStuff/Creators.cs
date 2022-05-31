﻿using UnityUtility.Pool;

#if INCLUDE_AUDIO
namespace UnityUtility.Sound.SoundStuff
{
    public class DynamicMusSourceCreator : IObjectFactory<MusicInfo>
    {
        public MusicInfo Create()
        {
            return ComponentUtility.CreateInstance<MusicInfo>();
        }
    }

    public class DynamicSndSourceCreator : IObjectFactory<SoundInfo>
    {
        public SoundInfo Create()
        {
            return ComponentUtility.CreateInstance<SoundInfo>();
        }
    }
}
#endif
