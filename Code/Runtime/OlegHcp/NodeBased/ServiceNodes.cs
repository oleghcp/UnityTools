using System;
using OlegHcp.NodeBased.Service;

namespace OlegHcp.NodeBased
{
    [Serializable]
    internal sealed class HubNode : RawNode
    {
        internal override NodeType NodeType => NodeType.Hub;
    }

    [Serializable]
    internal sealed class ExitNode : RawNode
    {
        internal override NodeType NodeType => NodeType.Exit;
    }

    [Serializable]
    internal sealed class CommonNode : RawNode
    {
        internal override NodeType NodeType => NodeType.Common;
    }
}
