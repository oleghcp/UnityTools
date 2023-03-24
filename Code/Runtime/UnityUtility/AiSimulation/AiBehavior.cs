using UnityEngine;
using UnityUtility.Engine;

namespace UnityUtility.AiSimulation
{
    [DisallowMultipleComponent]
    [AddComponentMenu(nameof(UnityUtility) + "/Ai Behavior")]
    public class AiBehavior : MonoBehaviour
    {
        [SerializeField]
        private AiBehaviorSet _behaviorSet;

        private bool _raw = true;
        private AiBehaviorSet _behaviorSetInstance;

        public PermanentState PermanentState => GetBehaviorSet().PermanentState;

#if UNITY_EDITOR
        internal AiBehaviorSet BehaviorSetInstance => _behaviorSetInstance;
#endif

        private void OnDestroy()
        {
            if (_behaviorSetInstance != null)
                _behaviorSetInstance.Destroy();
        }

        private void Update()
        {
            GetBehaviorSet().Refresh(Time.deltaTime);
        }

        private AiBehaviorSet GetBehaviorSet()
        {
            if (_raw)
            {
                _behaviorSetInstance = _behaviorSet.Install();
                _behaviorSetInstance.SetUp(gameObject);
                _raw = false;
            }

            return _behaviorSetInstance;
        }
    }
}
