#if UNITY_2019_3_OR_NEWER
using UnityEngine;

namespace UnityUtility.AiSimulation
{
    [CreateAssetMenu(menuName = nameof(UnityUtility) + "/Ai/Common Condition")]
    internal class CommonCondition : ScriptableObject
    {
        [SerializeReference]
        private StateCondition[] _conditions;

        public bool Satisfied(PermanentState permanentState)
        {
            return ConditionUtility.All(_conditions, permanentState);
        }
    }
}
#endif
