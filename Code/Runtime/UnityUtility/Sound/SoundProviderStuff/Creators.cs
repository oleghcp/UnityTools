namespace UnityUtility.Sound.SoundProviderStuff
{
    public class DynamicMusSourceCreator : IObjectCreator<MusObject>
    {
        public MusObject Create()
        {
            return ComponentUtility.CreateInstance<MusObject>();
        }
    }

    public class DynamicSndSourceCreator : IObjectCreator<SndObject>
    {
        public SndObject Create()
        {
            return ComponentUtility.CreateInstance<SndObject>();
        }
    }
}
