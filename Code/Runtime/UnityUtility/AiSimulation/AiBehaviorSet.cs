using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Inspector;

namespace UnityUtility.AiSimulation
{
    public class AiBehaviorSet : ScriptableObject
    {
        #region Inspector
        [SerializeReference, ReferenceSelection]
        private PermanentState _permanentState;

        [Space]
        [SerializeReference, ReferenceSelection]
        private BehaviorState[] _states;
        #endregion

        private GameObject _gameObject;
        private BehaviorState _currentState;

        public PermanentState PermanentState => _permanentState;

        public GameObject GameObject => _gameObject;

        internal void SetUp(GameObject gameObject)
        {
            _gameObject = gameObject;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_permanentState == null)
            {
                Debug.LogError("Permanent State not set.");
                return;
            }
#endif

            _permanentState.SetUp(this);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_states.Length == 0)
            {
                Debug.LogError("State list is empty.");
                return;
            }
#endif
            _states.ForEach(item => item.SetUp(this));
            _currentState = _states[^1];
            _currentState.OnBegin();
        }

        private void OnDestroy()
        {
            _states.ForEach(item => item.OnDestroy());
        }

        internal void Refresh(float deltaTime)
        {
            _permanentState.Refresh(deltaTime);

            for (int i = 0; i < _states.Length; i++)
            {
                if (_states[i].Available())
                {
                    if (_states[i] != _currentState)
                    {
                        _currentState.OnEnd();
                        _currentState = _states[i];
                        _currentState.OnBegin();
                    }

                    break;
                }
            }

            _currentState.Refresh(deltaTime);
        }

        public T GetComponent<T>()
        {
            return _gameObject.GetComponent<T>();
        }
    }
}
