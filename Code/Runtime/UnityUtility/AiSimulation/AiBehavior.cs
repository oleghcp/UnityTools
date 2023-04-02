using UnityEngine;
using UnityUtility.Engine;

namespace UnityUtility.AiSimulation
{
    internal interface IStateSet
    {
        PermanentState PermanentState { get; }

#if UNITY_EDITOR
        StateStatus Status { get; }
        object CurrentState { get; }
        object PrevState { get; }
#endif

        void SetUp(GameObject gameObject);
        void Destroy();
        void Refresh(float deltaTime);
        void Play();
        void Stop();
    }

    public enum StateStatus : byte
    {
        Running,
        Complete,
    }

    [DisallowMultipleComponent]
    [AddComponentMenu(nameof(UnityUtility) + "/Ai Behavior")]
    public class AiBehavior : MonoBehaviour
    {
        [SerializeField]
        private AiStateSet _behaviorSet;
        [SerializeField]
        private bool _playAutomatically = true;

        private bool _initialized;
        private IStateSet _behaviorSetInstance;
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
        internal StateStatus Status => _behaviorSetInstance.Status;
        internal object CurrentState => _behaviorSetInstance.CurrentState;
        internal object PrevState => _behaviorSetInstance.PrevState;
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
