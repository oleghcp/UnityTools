using System;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class StateFinalizer
    {
        public abstract void OnComlete(AiBehaviorSet owner);
    }
}
