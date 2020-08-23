using System;
using UnityEngine;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class NegativeIntIdGenerator : IIdGenerator<int>
    {
        [SerializeField, HideInInspector]
        private int m_lastId;

        public int LastID
        {
            get { return m_lastId; }
        }

        public NegativeIntIdGenerator() { }

        public NegativeIntIdGenerator(int startId)
        {
            m_lastId = startId;
        }

        public int GetNewId()
        {
            return --m_lastId;
        }
    }
}
