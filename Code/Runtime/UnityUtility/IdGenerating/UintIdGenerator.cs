using System;
using UnityEngine;
using UnityEngine.Scripting;

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

        [Preserve]
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
