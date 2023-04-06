using System;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.NodeBased
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

        //public override TState CreateState<TState>()
        //{
        //    return null;
        //}
    }

    [Serializable]
    internal sealed class CommonNode : RawNode
    {
        internal override NodeType NodeType => NodeType.Common;
    }
}
