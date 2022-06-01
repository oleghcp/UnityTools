using UnityUtility.Pool;

#if !UNITY_2019_1_OR_NEWER || INCLUDE_AUDIO
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
