using System;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField, HideInInspector, FormerlySerializedAs("_lastId")]
        protected T LastId;

        public T LastID => LastId;

        public IdGenerator(T startId)
        {
            LastId = startId;
        }

        public abstract T GetNewId();
    }
}
