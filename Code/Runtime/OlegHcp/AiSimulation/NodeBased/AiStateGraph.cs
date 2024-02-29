using System;
using OlegHcp.Inspector;
using OlegHcp.NodeBased;
using UnityEngine;

namespace OlegHcp.AiSimulation.NodeBased
{
    [CreateAssetMenu(menuName = nameof(OlegHcp) + "/Ai/State Graph")]
    public class AiStateGraph : Graph<BehaviorState>, IStateSet
    {
        [SerializeReference, ReferenceSelection]
        private PermanentState _permanentState;

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

            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].OnDestroy();
            }
        }

        public override Type GetConditionRootType()
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

            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].SetUp(_permanentState, gameObject);
            }
        }

        void IStateSet.Refresh(float deltaTime)
        {
            _permanentState?.Refresh(deltaTime);

            if (_currentState == null)
                return;

            if (_currentState.Interruptible || _status == StateStatus.Complete)
            {
                if (!UpdateState(_currentState))
                    UpdateState(EnumerateFromAny());
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
            if (Nodes.Count > 0)
            {
                _currentState = RootNode;
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

        private bool UpdateState(IEnumerableNode<BehaviorState> next)
        {
            foreach (TransitionInfo<BehaviorState> item in next)
            {
                if (item.Available(_permanentState))
                {
#if UNITY_EDITOR
                    _prevState = _currentState;
#endif
                    _currentState.OnEnd();
                    _currentState = item.NextNode;
                    _status = StateStatus.Running;
                    _currentState.OnBegin();

                    return true;
                }
            }

            return false;
        }
    }
}
