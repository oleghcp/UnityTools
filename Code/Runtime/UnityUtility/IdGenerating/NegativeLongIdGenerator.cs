namespace UU.IdGenerating
{
    public class NegativeLongIdGenerator : IdGenerator<long>
    {
        private long m_lastId;

        public NegativeLongIdGenerator() { }

        public NegativeLongIdGenerator(long startId)
        {
            m_lastId = startId;
        }

        public long LastID
        {
            get { return m_lastId; }
        }

        public long GetNewId()
        {
            return --m_lastId;
        }
    }
}
