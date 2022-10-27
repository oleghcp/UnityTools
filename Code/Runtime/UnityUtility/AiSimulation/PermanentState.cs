#if UNITY_2019_3_OR_NEWER
using System;
using UnityEngine;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class PermanentState
    {
        private AiBehaviorSet _behaviorSet;

        public AiBehaviorSet Owner => _behaviorSet;
#pragma warning disable IDE1006
        public GameObject gameObject => _behaviorSet.gameObject;
        public Transform transform => _behaviorSet.transform;
#pragma warning restore IDE1006

        internal void SetUp(AiBehaviorSet behaviorSet)
        {
            _behaviorSet = behaviorSet;
            OnSetUp();
        }

        protected virtual void OnSetUp() { }
        public virtual void OnDestroy() { }
        public abstract void Refresh(float deltaTime);

        public T GetComponent<T>()
        {
            return _behaviorSet.GetComponent<T>();
        }
    }
}
#endif
