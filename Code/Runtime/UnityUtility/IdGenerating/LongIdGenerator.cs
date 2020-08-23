using System;
using UnityEngine;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class LongIdGenerator : IIdGenerator<long>
    {
        [SerializeField, HideInInspector]
        private long m_lastId;

        public long LastID
        {
            get { return m_lastId; }
        }

        public LongIdGenerator() { }

        public LongIdGenerator(long startId)
        {
            m_lastId = startId;
        }

        public long GetNewId()
        {
            return ++m_lastId;
        }
    }
}