using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.Inspector;

namespace UnityUtility.AiSimulation.Simple
{
    [CreateAssetMenu(menuName = nameof(UnityUtility) + "/Ai/State Set")]
    public class AiStateSet : ScriptableObject, IStateSet
    {
        [SerializeReference, ReferenceSelection]
        private PermanentState _permanentState;

        [Space]
        [SerializeReference, ReferenceSelection]
        private BehaviorState[] _states;

        private StateStatus _status;
        private BehaviorState _currentState;

#if UNITY_EDITOR
        private BehaviorState _prevState;
#endif

        public PermanentState PermanentState => _permanentState;

#if UNITY_EDITOR
        StateStatus IStateSet.Status => _status;
        object IStateSet.CurrentState => _currentState;
        object IStateSet.PrevState => _prevState;
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

        void IStateSet.SetUp(GameObject gameObject)
        {
            _permanentState?.SetUp(gameObject);

            for (int i = 0; i < _states.Length; i++)
            {
                _states[i].SetUp(_permanentState, gameObject);
            }
        }

        void IStateSet.Refresh(float deltaTime)
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
                        _status = StateStatus.Running;
                        _currentState.OnBegin();
                    }

                    break;
                }
            }

            if (_currentState != null && _status == StateStatus.Running)
            {
                _status = _currentState.Refresh(deltaTime);

                if (_status == StateStatus.Complete)
                {
                    for (int i = 0; i < _currentState.Finalizers.Length; i++)
                    {
                        _currentState.Finalizers[i].OnComlete(_permanentState);
                    }
                }
            }
        }

        void IStateSet.Play()
        {
            if (_states.Length > 0)
            {
                _currentState = _states.FromEnd(0);
                _currentState.OnBegin();
            }
        }

        void IStateSet.Stop()
        {
            _currentState?.OnEnd();
        }

        void IStateSet.Destroy()
        {
            Destroy(this);
        }
    }
}
