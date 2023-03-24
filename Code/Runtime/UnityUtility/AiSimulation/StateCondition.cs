#if UNITY_2019_3_OR_NEWER
using System;
using UnityEngine;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class StateCondition
    {
        [SerializeField, HideInInspector]
        private bool _not;

#if UNITY_EDITOR
        internal static string NotFieldName => nameof(_not);
#endif

        internal bool Check(PermanentState permanentState)
        {
            return Satisfied(permanentState) != _not;
        }

        protected abstract bool Satisfied(PermanentState permanentState);
    }

    [Serializable]
    public abstract class StateCondition<T> : StateCondition where T : PermanentState
    {
        protected sealed override bool Satisfied(PermanentState permanentState)
        {
            return Satisfied((T)permanentState);
        }

        protected abstract bool Satisfied(T permanentState);
    }
}
#endif
