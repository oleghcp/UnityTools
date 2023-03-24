#if UNITY_2019_3_OR_NEWER
using System;
using UnityEngine;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    internal class Selection : StateCondition
    {
        [SerializeReference]
        private StateCondition[] _conditions;

        protected override bool Satisfied(PermanentState permanentState)
        {
            return ConditionUtility.Any(_conditions, permanentState);
        }
    }
}
#endif
