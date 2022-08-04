#if UNITY_2019_3_OR_NEWER
using System;
using UnityEngine;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public class Selection : StateCondition
    {
        [SerializeReference]
        private StateCondition[] _conditions;

        protected override bool Satisfied(AiBehaviorSet owner)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (_conditions[i].Check(owner))
                    return true;
            }

            return false;
        }
    }
}
#endif
