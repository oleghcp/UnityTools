#if UNITY_2019_3_OR_NEWER
using System.Runtime.CompilerServices;

namespace UnityUtility.NodeBased
{
    internal static class NodeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ServiceNode(this RawNode self)
        {
            return self.NodeType != NodeType.Real;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RealNode(this RawNode self)
        {
            return self.NodeType == NodeType.Real;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ServiceNode(this NodeType self)
        {
            return self != NodeType.Real;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RealNode(this NodeType self)
        {
            return self == NodeType.Real;
        }
    }
}
#endif
