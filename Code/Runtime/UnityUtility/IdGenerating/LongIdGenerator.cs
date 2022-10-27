using System;
using UnityEngine.Scripting;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class LongIdGenerator : IdGenerator<long>
    {
        [Preserve]
        public LongIdGenerator() : base(0) { }
        public LongIdGenerator(long startId) : base(startId) { }

        public override long GetNewId()
        {
            return ++LastId;
        }
    }
}
