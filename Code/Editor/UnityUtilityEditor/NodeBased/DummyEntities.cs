using UnityUtility.NodeBased;

namespace UnityUtilityEditor.NodeBased
{
    internal abstract class DummyGrapth : Graph<DummyNode> { }

    internal abstract class DummyNode : Node<Transition> { }
}
