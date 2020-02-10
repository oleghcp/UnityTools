namespace UU.IDGenerating
{
    public class UlongIDGenerator : IDGenerator<ulong>
    {
        private ulong m_lastId;

        public UlongIDGenerator() { }

        public UlongIDGenerator(ulong startId)
        {
            m_lastId = startId;
        }

        public ulong LastID
        {
            get { return m_lastId; }
        }

        public ulong GetNewId()
        {
            return ++m_lastId;
        }
    }
}
