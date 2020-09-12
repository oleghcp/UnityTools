using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class UlongIdGenerator : IIdGenerator<ulong>
    {
        [SerializeField, HideInInspector]
        private ulong m_lastId;

        public ulong LastID
        {
            get { return m_lastId; }
        }

        [Preserve]
        public UlongIdGenerator() { }

        public UlongIdGenerator(ulong startId)
        {
            m_lastId = startId;
        }

        public ulong GetNewId()
        {
            return ++m_lastId;
        }
    }
}
