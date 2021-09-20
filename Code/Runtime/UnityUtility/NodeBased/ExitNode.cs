#if UNITY_2019_3_OR_NEWER
using System;

namespace UnityUtility.NodeBased
{
    [Serializable]
    internal sealed class ExitNode : RawNode
    {
        internal override NodeType NodeType => NodeType.Exit;

        public override TState CreateState<TState>()
        {
            return null;
        }
    }
}
#endif
