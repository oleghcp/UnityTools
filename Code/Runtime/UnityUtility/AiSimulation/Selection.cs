﻿#if UNITY_2019_3_OR_NEWER
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

        public override bool Satisfied(AiBehaviorSet owner)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (_conditions[i].Satisfied(owner))
                    return true;
            }

            return false;
        }
    }
}
#endif
