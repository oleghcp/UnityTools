using System;
using UnityEngine;
using UnityEngine.Scripting;

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

        [Preserve]
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
