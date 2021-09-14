namespace UnityUtility.NodeBased
{
    internal sealed class ExitNode : RawNode
    {
        public override TState CreateState<TState>()
        {
            return null;
        }
    }
}
