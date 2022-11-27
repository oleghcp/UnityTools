#if UNITY_2019_3_OR_NEWER
using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.Inspector;

#pragma warning disable UNT0014
namespace UnityUtility.AiSimulation
{
    [CreateAssetMenu(menuName = nameof(UnityUtility) + "/Ai/Behavior Set")]
    public class AiBehaviorSet : ScriptableObject
    {
        [SerializeReference, ReferenceSelection]
        private PermanentState _permanentState;

        [Space]
        [SerializeReference, ReferenceSelection]
        private BehaviorState[] _states;

        private GameObject _gameObject;
        private Transform _transform;
        private BehaviorState _currentState;
#if UNITY_EDITOR
        private BehaviorState _prevState;
#endif

        public PermanentState PermanentState => _permanentState;

#pragma warning disable IDE1006
        public GameObject gameObject => _gameObject;
        public Transform transform => _transform;
#pragma warning restore IDE1006

#if UNITY_EDITOR
        internal BehaviorState CurrentState => _currentState;
        internal BehaviorState PrevState => _prevState;
#endif

        internal void SetUp(GameObject gameObject)
        {
            _gameObject = gameObject;
            _transform = gameObject.transform;

            _permanentState?.SetUp(this);

            if (_states.Length == 0)
                return;

            for (int i = 0; i < _states.Length; i++)
            {
                _states[i].SetUp(this);
            }

            _currentState = _states.FromEnd(0);
            _currentState.OnBegin();
        }

        private void OnDestroy()
        {
            _permanentState?.OnDestroy();

            for (int i = 0; i < _states.Length; i++)
            {
                _states[i].OnDestroy();
            }
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

        public T GetComponent<T>()
        {
            return _gameObject.GetComponent<T>();
        }
    }
}
#endif
