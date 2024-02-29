using System;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using UnityEngine;

namespace OlegHcp.AiSimulation.Simple
{
    [CreateAssetMenu(menuName = nameof(OlegHcp) + "/Ai/State Set")]
    public class AiStateSet : ScriptableObject, IStateSet
    {
        [SerializeReference, ReferenceSelection]
        private PermanentState _permanentState;

        [Space]
        [SerializeReference]
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

        public virtual Type GetStateRootType()
        {
            return typeof(BehaviorState);
        }

        public virtual Type GetConditionRootType()
        {
            return typeof(StateCondition);
        }

        public virtual Type GetCompleteHandlerRootType()
        {
            return typeof(CompleteHandler);
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

            if (_currentState == null)
                return;

            if (_currentState.Interruptible || _status == StateStatus.Complete)
            {
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
            }

            if (_status == StateStatus.Running)
            {
                _status = _currentState.Refresh(deltaTime);

                if (_status == StateStatus.Complete)
                {
                    for (int i = 0; i < _currentState.CompleteHandlers.Length; i++)
                    {
                        _currentState.CompleteHandlers[i].OnComlete(_permanentState);
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
