namespace UU.IDGenerating
{
    public class IntIDGenerator : IDGenerator<int>
    {
        private int m_lastId;

        public IntIDGenerator() { }

        public IntIDGenerator(int startId)
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