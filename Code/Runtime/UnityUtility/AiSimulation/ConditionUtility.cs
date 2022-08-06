#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.AiSimulation
{
    internal static class ConditionUtility
    {
        public static bool All(StateCondition[] conditions, AiBehaviorSet owner)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (!conditions[i].Check(owner))
                    return false;
            }

            return true;
        }

        public static bool Any(StateCondition[] conditions, AiBehaviorSet owner)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].Check(owner))
                    return true;
            }

            return false;
        }
    }
}
#endif
