using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.Inspector;

namespace UnityUtility.AiSimulation
{
    [CreateAssetMenu(menuName = nameof(UnityUtility) + "/Ai/Behavior Set")]
    internal class AiBehaviorSet : ScriptableObject
    {
        [SerializeReference, ReferenceSelection]
        private PermanentState _permanentState;

        [Space]
        [SerializeReference, ReferenceSelection]
        private BehaviorState[] _states;

        private BehaviorState _currentState;
#if UNITY_EDITOR
        private BehaviorState _prevState;
#endif

        public PermanentState PermanentState => _permanentState;

#if UNITY_EDITOR
        public BehaviorState CurrentState => _currentState;
        public BehaviorState PrevState => _prevState;
#endif

        private void OnDestroy()
        {
            _permanentState?.OnDestroy();
            _currentState?.OnEnd();

            for (int i = 0; i < _states.Length; i++)
            {
                _states[i].OnDestroy();
            }
        }

        public void SetUp(GameObject gameObject)
        {
            _permanentState?.SetUp(gameObject);

            for (int i = 0; i < _states.Length; i++)
            {
                _states[i].SetUp(_permanentState, gameObject);
            }
        }

        public void Refresh(float deltaTime)
        {
            _permanentState?.Refresh(deltaTime);

            for (int i = 0; i < _states.Length; i++)
            {
                if (_states[i].Available())
                {
                    if (_states[i] != _currentState)
                    {
#if UNITY_EDITOR
                        _prevState = _currentState;
#endif
                        _currentState.OnEnd();
                        _currentState = _states[i];
                        _currentState.OnBegin();
                    }

                    break;
                }
            }

            _currentState?.Refresh(deltaTime);
        }

        public void Play()
        {
            if (_states.Length > 0)
            {
                _currentState = _states.FromEnd(0);
                _currentState.OnBegin();
            }
        }

        public void Stop()
        {
            _currentState?.OnEnd();
        }
    }
}
