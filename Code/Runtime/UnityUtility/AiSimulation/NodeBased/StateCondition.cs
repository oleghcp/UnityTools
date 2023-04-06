using System;
using UnityUtility.NodeBased;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.AiSimulation.NodeBased
{
    [Serializable]
    public abstract class StateCondition : Condition
    {
        protected sealed override bool Satisfied(RawNode _, object data)
        {
            return Satisfied((PermanentState)data);
        }

        protected abstract bool Satisfied(PermanentState permanentState);
    }


    [Serializable]
    public abstract class StateCondition<TPermanent> : StateCondition
        where TPermanent : PermanentState
    {
        protected sealed override bool Satisfied(PermanentState permanentState)
        {
            return Satisfied((TPermanent)permanentState);
        }

        protected abstract bool Satisfied(TPermanent permanentState);
    }
}
