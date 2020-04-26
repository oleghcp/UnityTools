namespace UnityUtility.IdGenerating
{
    public class UlongIdGenerator : IIdGenerator<ulong>
    {
        private ulong m_lastId;

        public UlongIdGenerator() { }

        public UlongIdGenerator(ulong startId)
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
