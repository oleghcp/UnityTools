using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class NegativeLongIdGenerator : IIdGenerator<long>
    {
        [SerializeField, HideInInspector]
        private long m_lastId;

        public long LastID
        {
            get { return m_lastId; }
        }

        [Preserve]
        public NegativeLongIdGenerator() { }

        public NegativeLongIdGenerator(long startId)
        {
            m_lastId = startId;
        }

        public long GetNewId()
        {
            return --m_lastId;
        }
    }
}
