namespace UnityUtility.NodeBased
{
    internal static class NodeExtensions
    {
        public static bool ServiceNode(this RawNode self)
        {
            return self.NodeType != NodeType.Real;
        }

        public static bool RealNode(this RawNode self)
        {
            return self.NodeType == NodeType.Real;
        }

        public static bool ServiceNode(this NodeType self)
        {
            return self != NodeType.Real;
        }

        public static bool RealNode(this NodeType self)
        {
            return self == NodeType.Real;
        }
    }
}
