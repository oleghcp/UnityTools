namespace UnityUtility.Sound.SoundStuff
{
    public class DynamicMusSourceCreator : IObjectCreator<MusicInfo>
    {
        public MusicInfo Create()
        {
            return ComponentUtility.CreateInstance<MusicInfo>();
        }
    }

    public class DynamicSndSourceCreator : IObjectCreator<SoundInfo>
    {
        public SoundInfo Create()
        {
            return ComponentUtility.CreateInstance<SoundInfo>();
        }
    }
}
