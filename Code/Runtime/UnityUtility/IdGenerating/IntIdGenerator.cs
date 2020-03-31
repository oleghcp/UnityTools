namespace UU.IdGenerating
{
    public class IntIdGenerator : IdGenerator<int>
    {
        private int m_lastId;

        public IntIdGenerator() { }

        public IntIdGenerator(int startId)
        {
            m_lastId = startId;
        }

        public int LastID
        {
            get { return m_lastId; }
        }

        public int GetNewId()
        {
            return ++m_lastId;
        }
    }
}