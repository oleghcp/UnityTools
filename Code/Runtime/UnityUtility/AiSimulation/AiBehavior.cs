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

        private bool _initialized;
        private AiBehaviorSet _behaviorSetInstance;

        public PermanentState PermanentState
        {
            get
            {
                UpdateBehaviorSet();
                return _behaviorSetInstance.PermanentState;
            }
        }

#if UNITY_EDITOR
        internal bool Initialized => _initialized;
        internal BehaviorState CurrentState => _behaviorSetInstance.CurrentState;
        internal BehaviorState PrevState => _behaviorSetInstance.PrevState;
#endif

        private void OnDestroy()
        {
            if (_initialized)
                _behaviorSetInstance.Destroy();
        }

        private void Update()
        {
            UpdateBehaviorSet();
            _behaviorSetInstance.Refresh(Time.deltaTime);
        }

        private void UpdateBehaviorSet()
        {
            if (_initialized)
                return;

            _initialized = true;
            _behaviorSetInstance = _behaviorSet.Install();
            _behaviorSetInstance.SetUp(gameObject);
        }
    }
}
