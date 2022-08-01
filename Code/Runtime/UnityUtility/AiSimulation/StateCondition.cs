using System;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class StateCondition
    {
        public abstract bool Satisfy(AiBehaviorSet owner);
    }
}
