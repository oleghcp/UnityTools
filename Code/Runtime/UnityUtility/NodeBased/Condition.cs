using System;
using UnityUtility.Collections;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.NodeBased
{
    [Serializable]
    public abstract class Condition
    {
        public abstract bool Satisfied(RawNode from, object data);

        public virtual Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState
        {
            throw new NotImplementedException();
        }
    }
}
