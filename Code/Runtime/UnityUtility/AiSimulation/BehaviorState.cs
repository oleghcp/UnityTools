using System;
using UnityEngine;
using UnityUtility.Inspector;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class BehaviorState
    {
        [SerializeReference]
        private StateCondition[] _conditions;

        private GameObject _gameObject;
        private Transform _transform;
        private PermanentState _permanentState;

#pragma warning disable IDE1006
        protected GameObject gameObject => _gameObject;
        protected Transform transform => _transform;
#pragma warning restore IDE1006

        protected PermanentState PermanentState => _permanentState;

        internal void SetUp(PermanentState permanentState, GameObject gameObject)
        {
            _gameObject = gameObject;
            _transform = gameObject.transform;
            _permanentState = permanentState;
            OnSetUp();
        }

        public bool Available()
        {
            return ConditionUtility.All(_conditions, _permanentState);
        }

        protected virtual void OnSetUp() { }
        public virtual void OnDestroy() { }
        public virtual void OnBegin() { }
        public virtual void OnEnd() { }
        public abstract void Refresh(float deltaTime);

        protected T GetComponent<T>()
        {
            return _gameObject.GetComponent<T>();
        }
    }

    [Serializable]
    public abstract class CompletableState : BehaviorState
    {
        [SerializeReference, ReferenceSelection]
        private StateFinalizer[] _finalizers;

        public sealed override void Refresh(float deltaTime)
        {
            if (OnRefresh(deltaTime) == Status.Complete)
            {
                for (int i = 0; i < _finalizers.Length; i++)
                {
                    _finalizers[i].OnComlete(PermanentState);
                }
            }
        }

        protected abstract Status OnRefresh(float deltaTime);

        protected enum Status : byte
        {
            Running,
            Complete,
        }
    }
}
