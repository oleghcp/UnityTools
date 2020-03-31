namespace UnityUtility.IdGenerating
{
    public class NegativeIntIdGenerator : IdGenerator<int>
    {
        private int m_lastId;

        public NegativeIntIdGenerator() { }

        public NegativeIntIdGenerator(int startId)
        {
            m_lastId = startId;
        }

        public int LastID
        {
            get { return m_lastId; }
        }

        public int GetNewId()
        {
            return --m_lastId;
        }
    }
}
