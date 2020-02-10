using UU.Collections;

namespace UU.Sound.SoundProviderStuff
{
    public class DynamicMusSourceCreator : ObjectCreator<MusObject>
    {
        public MusObject Create()
        {
            return Script.CreateInstance<MusObject>();
        }
    }

    public class DynamicSndSourceCreator : ObjectCreator<SndObject>
    {
        public SndObject Create()
        {
            return Script.CreateInstance<SndObject>();
        }
    }
}
