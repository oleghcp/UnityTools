using System;
using UnityEngine.Scripting;

namespace UnityUtility.IdGenerating
{
    [Serializable]
    public class IntIdGenerator : IdGenerator<int>
    {
        [Preserve]
        public IntIdGenerator() : base(0) { }
        public IntIdGenerator(int startId) : base(startId) { }

        public override int GetNewId()
        {
            return ++LastId;
        }
    }
}
