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

        internal bool Check(AiBehaviorSet owner)
        {
            return Satisfied(owner) != _not;
        }

        protected abstract bool Satisfied(AiBehaviorSet owner);
    }
}
#endif
