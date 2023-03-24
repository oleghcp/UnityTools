using System;
using UnityEngine;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    internal class Common : StateCondition
    {
        [SerializeField]
        private CommonCondition _common;

        protected override bool Satisfied(PermanentState permanentState)
        {
            return _common.Satisfied(permanentState);
        }
    }
}
