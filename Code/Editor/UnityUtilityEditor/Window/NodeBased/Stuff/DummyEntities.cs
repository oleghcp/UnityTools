using UnityUtility.NodeBased;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal abstract class DummyGrapth : Graph<DummyNode> { }
    internal abstract class DummyNode : Node<Transition> { }
}
