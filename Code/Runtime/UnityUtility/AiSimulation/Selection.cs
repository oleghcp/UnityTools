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
