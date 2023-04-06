using System;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class CompleteHandler
    {
        public abstract void OnComlete(PermanentState permanentState);
    }

    public abstract class CompleteHandler<T> : CompleteHandler where T : PermanentState
    {
        public sealed override void OnComlete(PermanentState permanentState)
        {
            OnComlete((T)permanentState);
        }

        public abstract void OnComlete(T permanentState);
    }
}
