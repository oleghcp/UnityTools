using System;
using UnityEngine;
using UnityUtility.Inspector;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public class Selection : StateCondition
    {
        [SerializeReference, ReferenceSelection]
        private StateCondition[] _conditions;

        public override bool Satisfy(AiBehaviorSet owner)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (_conditions[i].Satisfy(owner))
                    return true;
            }

            return false;
        }
    }
}
