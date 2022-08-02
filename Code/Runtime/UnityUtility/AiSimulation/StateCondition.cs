#if UNITY_2019_3_OR_NEWER
using System;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class StateCondition
    {
        public abstract bool Satisfied(AiBehaviorSet owner);
    }
}
#endif
