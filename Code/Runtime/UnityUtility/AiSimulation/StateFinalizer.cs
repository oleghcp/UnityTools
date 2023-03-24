using System;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class StateFinalizer
    {
        public abstract void OnComlete(PermanentState permanentState);
    }

    public abstract class StateFinalizer<T> : StateFinalizer where T : PermanentState
    {
        public sealed override void OnComlete(PermanentState permanentState)
        {
            OnComlete((T)permanentState);
        }

        public abstract void OnComlete(T permanentState);
    }
}
