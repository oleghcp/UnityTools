namespace UnityUtility.IdGenerating
{
    public class LongIdGenerator : IdGenerator<long>
    {
        private long m_lastId;

        public LongIdGenerator() { }

        public LongIdGenerator(long startId)
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