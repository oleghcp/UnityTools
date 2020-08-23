using System;
using UnityEngine;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class UintIdGenerator : IIdGenerator<uint>
    {
        [SerializeField, HideInInspector]
        private uint m_lastId;

        public uint LastID
        {
            get { return m_lastId; }
        }

        public UintIdGenerator() { }

        public UintIdGenerator(uint startId)
        {
            m_lastId = startId;
        }

        public uint GetNewId()
        {
            return ++m_lastId;
        }
    }
}
