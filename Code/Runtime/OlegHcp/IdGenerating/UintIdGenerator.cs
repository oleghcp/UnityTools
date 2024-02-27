using System;
using UnityEngine.Scripting;

namespace OlegHcp.IdGenerating
{
    [Serializable]
    public class UintIdGenerator : IdGenerator<uint>
    {
        [Preserve]
        public UintIdGenerator() : base(0) { }
        public UintIdGenerator(uint startId) : base(startId) { }

        public override uint GetNewId()
        {
            return ++LastId;
        }
    }
}
