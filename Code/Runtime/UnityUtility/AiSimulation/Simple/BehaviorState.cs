using System;
using UnityEngine;

namespace UnityUtility.AiSimulation.Simple
{
    [Serializable]
    public abstract class BehaviorState
    {
        [SerializeReference]
        private StateCondition[] _conditions;
        [SerializeReference]
        private CompleteHandler[] _onComlete;

        private GameObject _gameObject;
        private Transform _transform;
        private PermanentState _permanentState;

#pragma warning disable IDE1006
        protected GameObject gameObject => _gameObject;
        protected Transform transform => _transform;
#pragma warning restore IDE1006

        protected PermanentState PermanentState => _permanentState;
        internal CompleteHandler[] CompleteHandlers => _onComlete;

        internal void SetUp(PermanentState permanentState, GameObject gameObject)
        {
            _gameObject = gameObject;
            _transform = gameObject.transform;
            _permanentState = permanentState;
            OnSetUp();
        }

        public bool Available()
        {
            return StateCondition.All(_conditions, _permanentState);
        }

        protected virtual void OnSetUp() { }
        public virtual void OnDestroy() { }
        public virtual void OnBegin() { }
        public virtual void OnEnd() { }
        public abstract StateStatus Refresh(float deltaTime);

        protected T GetComponent<T>()
        {
            return _permanentState.GetComponent<T>();
        }
    }

    [Serializable]
    public abstract class BehaviorState<T> : BehaviorState where T : PermanentState
    {
        protected new T PermanentState => (T)base.PermanentState;
    }
}
