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

        internal static bool All(StateCondition[] conditions, PermanentState permanentState)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (!conditions[i].Check(permanentState))
                    return false;
            }

            return true;
        }

        internal static bool Any(StateCondition[] conditions, PermanentState permanentState)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].Check(permanentState))
                    return true;
            }

            return false;
        }
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

    [Serializable]
    internal class Any : StateCondition
    {
        [SerializeReference]
        private StateCondition[] _conditions;

        protected override bool Satisfied(PermanentState permanentState)
        {
            return Any(_conditions, permanentState);
        }
    }

    [Serializable]
    internal class All : StateCondition
    {
        [SerializeReference]
        private StateCondition[] _conditions;

        protected override bool Satisfied(PermanentState permanentState)
        {
            return All(_conditions, permanentState);
        }
    }
}
