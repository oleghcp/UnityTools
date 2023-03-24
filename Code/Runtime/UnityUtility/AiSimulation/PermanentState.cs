using System;
using UnityEngine;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class PermanentState
    {
        private GameObject _gameObject;
        private Transform _transform;

#pragma warning disable IDE1006
        public GameObject gameObject => _gameObject;
        public Transform transform => _transform;
#pragma warning restore IDE1006

        internal void SetUp(GameObject gameObject)
        {
            _gameObject = gameObject;
            _transform = gameObject.transform;
            OnSetUp();
        }

        protected virtual void OnSetUp() { }
        public virtual void OnDestroy() { }
        public abstract void Refresh(float deltaTime);

        public T GetComponent<T>()
        {
            return _gameObject.GetComponent<T>();
        }
    }
}
