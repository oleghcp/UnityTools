#if UNITY_2019_3_OR_NEWER
using System;

namespace UnityUtility.NodeBased
{
    [Serializable]
    internal sealed class HubNode : RawNode
    {
        internal override NodeType NodeType => NodeType.Hub;
    }
}
#endif
