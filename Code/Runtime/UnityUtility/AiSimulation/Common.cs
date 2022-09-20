#if UNITY_2019_3_OR_NEWER
using System;
using UnityEngine;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    internal class Common : StateCondition
    {
        [SerializeField]
        private CommonCondition _common;

        protected override bool Satisfied(AiBehaviorSet owner)
        {
            return _common.Satisfied(owner);
        }
    }
}
#endif
