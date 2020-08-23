using System;
using UnityEngine;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class IntIdGenerator : IIdGenerator<int>
    {
        [SerializeField, HideInInspector]
        private int m_lastId;

        public int LastID
        {
            get { return m_lastId; }
        }

        public IntIdGenerator() { }

        public IntIdGenerator(int startId)
        {
            m_lastId = startId;
        }

        public int GetNewId()
        {
            return ++m_lastId;
        }
    }
}