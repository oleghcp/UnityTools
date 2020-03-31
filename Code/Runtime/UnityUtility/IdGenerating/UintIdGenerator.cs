namespace UnityUtility.IdGenerating
{
    public class UintIdGenerator : IdGenerator<uint>
    {
        private uint m_lastId;

        public UintIdGenerator() { }

        public UintIdGenerator(uint startId)
        {
            m_lastId = startId;
        }

        public uint LastID
        {
            get { return m_lastId; }
        }

        public uint GetNewId()
        {
            return ++m_lastId;
        }
    }
}
