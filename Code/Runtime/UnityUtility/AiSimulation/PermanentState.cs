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
        public GameObject GameObject => _behaviorSet.GameObject;

        internal void SetUp(AiBehaviorSet behaviorSet)
        {
            _behaviorSet = behaviorSet;
            OnSetUp();
        }

        protected abstract void OnSetUp();
        public abstract void Refresh(float deltaTime);

        public T GetComponent<T>()
        {
            return _behaviorSet.GetComponent<T>();
        }
    }
}
#endif
