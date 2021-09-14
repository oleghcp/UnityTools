using System.Runtime.CompilerServices;

namespace UnityUtility.NodeBased
{
    internal static class NodeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ServiceNode(this RawNode self)
        {
            return self is HubNode || self is ExitNode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RealNode(this RawNode self)
        {
            return !self.ServiceNode();
        }
    }
}
