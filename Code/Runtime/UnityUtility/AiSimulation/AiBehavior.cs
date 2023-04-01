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
        [SerializeField]
        private bool _playAutomatically = true;

        private bool _initialized;
        private AiBehaviorSet _behaviorSetInstance;
        private bool _active;

        public bool Active => _active;

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
        internal BehaviorState.Status Status => _behaviorSetInstance.Status;
        internal BehaviorState CurrentState => _behaviorSetInstance.CurrentState;
        internal BehaviorState PrevState => _behaviorSetInstance.PrevState;
#endif

        private void Awake()
        {
            if (_playAutomatically)
                Play();
        }

        private void OnDestroy()
        {
            if (_initialized)
                _behaviorSetInstance.Destroy();
        }

        private void Update()
        {
            if (_active)
            {
                UpdateBehaviorSet();
                _behaviorSetInstance.Refresh(Time.deltaTime);
            }
        }

        public void Play()
        {
            if (_active)
                return;

            _active = true;
            UpdateBehaviorSet();
            _behaviorSetInstance.Play();
        }

        public void Stop()
        {
            if (_active)
            {
                _active = false;
                UpdateBehaviorSet();
                _behaviorSetInstance.Stop();
            }
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
