using System;
using UnityEngine.Scripting;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class UlongIdGenerator : IdGenerator<ulong>
    {
        [Preserve]
        public UlongIdGenerator() : base(0) { }
        public UlongIdGenerator(ulong startId) : base(startId) { }

        public override ulong GetNewId()
        {
            return ++_lastId;
        }
    }
}
