using System;

namespace OlegHcp.AiSimulation
{
    [Serializable]
    public abstract class CompleteHandler
    {
        public abstract void OnComplete(PermanentState permanentState);
    }

    public abstract class CompleteHandler<T> : CompleteHandler where T : PermanentState
    {
        public sealed override void OnComplete(PermanentState permanentState)
        {
            OnComplete((T)permanentState);
        }

        public abstract void OnComplete(T permanentState);
    }
}
