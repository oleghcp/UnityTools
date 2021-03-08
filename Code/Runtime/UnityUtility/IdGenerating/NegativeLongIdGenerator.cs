using System;
using UnityEngine.Scripting;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class NegativeLongIdGenerator : IdGenerator<long>
    {
        [Preserve]
        public NegativeLongIdGenerator() : base(0) { }
        public NegativeLongIdGenerator(long startId) : base(startId) { }

        public override long GetNewId()
        {
            return --_lastId;
        }
    }
}
