namespace UU.IDGenerating
{
    public class UintIDGenerator : IDGenerator<uint>
    {
        private uint m_lastId;

        public UintIDGenerator() { }

        public UintIDGenerator(uint startId)
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
