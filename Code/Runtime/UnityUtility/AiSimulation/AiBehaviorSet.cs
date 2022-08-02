#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Inspector;

namespace UnityUtility.AiSimulation
{
    [CreateAssetMenu(menuName = nameof(UnityUtility) + "/Ai/Behavior Set")]
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
        private Transform _transform;
        private BehaviorState _currentState;

        public PermanentState PermanentState => _permanentState;

        public GameObject gameObject => _gameObject;
        public Transform transform => _transform;

        internal void SetUp(GameObject gameObject)
        {
            _gameObject = gameObject;
            _transform = gameObject.transform;

            _permanentState?.SetUp(this);

            if (_states.Length == 0)
                return;

            _states.ForEach(item => item.SetUp(this));
            _currentState = _states.FromEnd(0);
            _currentState.OnBegin();
        }

        private void OnDestroy()
        {
            _states.ForEach(item => item.OnDestroy());
        }

        internal void Refresh(float deltaTime)
        {
            _permanentState?.Refresh(deltaTime);

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

            _currentState?.Refresh(deltaTime);
        }

        public T GetComponent<T>()
        {
            return _gameObject.GetComponent<T>();
        }
    }
}
#endif
