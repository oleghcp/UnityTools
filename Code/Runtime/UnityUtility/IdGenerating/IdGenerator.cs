using System;
using UnityEngine;

namespace UnityUtility.IdGenerating
{
    public interface IIdGenerator<T>
    {
        T LastID { get; }
        T GetNewId();
    }

    [Serializable]
    public abstract class IdGenerator<T> : IIdGenerator<T>
    {
        [SerializeField, HideInInspector]
        protected T _lastId;

        public T LastID => _lastId;

        public IdGenerator(T startId)
        {
            _lastId = startId;
        }

        public abstract T GetNewId();
    }
}
