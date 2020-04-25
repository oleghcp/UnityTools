using UnityUtility.Collections;

namespace UnityUtility.Sound.SoundProviderStuff
{
    public class DynamicMusSourceCreator : ObjectCreator<MusObject>
    {
        public MusObject Create()
        {
            return ComponentUtility.CreateInstance<MusObject>();
        }
    }

    public class DynamicSndSourceCreator : ObjectCreator<SndObject>
    {
        public SndObject Create()
        {
            return ComponentUtility.CreateInstance<SndObject>();
        }
    }
}
