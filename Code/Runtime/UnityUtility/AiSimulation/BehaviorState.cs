#if UNITY_2019_3_OR_NEWER
using System;
using UnityEngine;
using UnityUtility.Inspector;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class BehaviorState
    {
        [SerializeReference, ReferenceSelection]
        private StateCondition[] _conditions;

        private AiBehaviorSet _behaviorSet;

        public AiBehaviorSet Owner => _behaviorSet;
        public GameObject GameObject => _behaviorSet.GameObject;

        internal void SetUp(AiBehaviorSet behaviorSet)
        {
            _behaviorSet = behaviorSet;
            OnSetUp();
        }

        public bool Available()
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (!_conditions[i].Satisfy(Owner))
                    return false;
            }

            return true;
        }

        protected virtual void OnSetUp() { }
        public virtual void OnDestroy() { }
        public virtual void OnBegin() { }
        public virtual void OnEnd() { }
        public abstract void Refresh(float deltaTime);

        public T GetComponent<T>()
        {
            return _behaviorSet.GetComponent<T>();
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
                    _finalizers[i].OnComlete(Owner);
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
#endif
