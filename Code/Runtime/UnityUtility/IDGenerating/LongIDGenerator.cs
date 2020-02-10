namespace UU.IDGenerating
{
    public class LongIDGenerator : IDGenerator<long>
    {
        private long m_lastId;

        public LongIDGenerator() { }

        public LongIDGenerator(long startId)
        {
            m_lastId = startId;
        }

        public long LastID
        {
            get { return m_lastId; }
        }

        public long GetNewId()
        {
            return ++m_lastId;
        }
    }
}