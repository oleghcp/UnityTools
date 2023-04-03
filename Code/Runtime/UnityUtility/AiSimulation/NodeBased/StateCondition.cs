using System;
using UnityUtility.NodeBased;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.AiSimulation.NodeBased
{
    [Serializable]
    public abstract class StateCondition : Condition
    {
        protected sealed override bool Satisfied(RawNode from, object data)
        {
            return Satisfied((BehaviorState)from, (PermanentState)data);
        }

        protected abstract bool Satisfied(BehaviorState from, PermanentState permanentState);
    }


    [Serializable]
    public abstract class StateCondition<TState, TPermanent> : StateCondition
        where TState : BehaviorState
        where TPermanent : PermanentState
    {
        protected sealed override bool Satisfied(BehaviorState from, PermanentState permanentState)
        {
            return Satisfied((TState)from, (TPermanent)permanentState);
        }

        protected abstract bool Satisfied(TState from, TPermanent permanentState);
    }
}
