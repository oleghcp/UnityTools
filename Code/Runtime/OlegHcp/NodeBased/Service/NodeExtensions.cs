namespace OlegHcp.NodeBased.Service
{
    internal static class NodeExtensions
    {
        public static bool IsServiceNode(this RawNode self)
        {
            return self.NodeType != NodeType.Regular;
        }

        public static bool IsServiceNode(this NodeType self)
        {
            return self != NodeType.Regular;
        }

        public static bool IsRegular(this RawNode self)
        {
            return self.NodeType == NodeType.Regular;
        }

        public static bool IsRegular(this NodeType self)
        {
            return self == NodeType.Regular;
        }
    }
}
