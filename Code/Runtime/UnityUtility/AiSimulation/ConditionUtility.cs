#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.AiSimulation
{
    internal static class ConditionUtility
    {
        public static bool All(StateCondition[] conditions, PermanentState permanentState)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (!conditions[i].Check(permanentState))
                    return false;
            }

            return true;
        }

        public static bool Any(StateCondition[] conditions, PermanentState permanentState)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].Check(permanentState))
                    return true;
            }

            return false;
        }
    }
}
#endif
