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
        private BehaviorState.Status _status;

        public PermanentState PermanentState => _permanentState;

#if UNITY_EDITOR
        public BehaviorState.Status Status => _status;
        public BehaviorState CurrentState => _currentState;
        public BehaviorState PrevState { get; private set; }
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
                        PrevState = _currentState;
#endif
                        _currentState.OnEnd();
                        _currentState = _states[i];
                        _status = BehaviorState.Status.Running;
                        _currentState.OnBegin();
                    }

                    break;
                }
            }

            if (_currentState != null && _status == BehaviorState.Status.Running)
            {
                _status = _currentState.Refresh(deltaTime);

                if (_status == BehaviorState.Status.Complete)
                {
                    for (int i = 0; i < _currentState.Finalizers.Length; i++)
                    {
                        _currentState.Finalizers[i].OnComlete(_permanentState);
                    }
                }
            }
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
