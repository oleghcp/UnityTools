using System;
using UnityEngine.Scripting;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class NegativeIntIdGenerator : IdGenerator<int>
    {
        [Preserve]
        public NegativeIntIdGenerator() : base(0) { }
        public NegativeIntIdGenerator(int startId) : base(startId) { }

        public override int GetNewId()
        {
            return --_lastId;
        }
    }
}
