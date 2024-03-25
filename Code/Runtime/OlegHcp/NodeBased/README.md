## Node Based

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/NodeBased1.png)

```csharp
using System;
using OlegHcp.NodeBased;

[Serializable]
public class ExampleNode : Node<ExampleNode>
{
    [UnityEngine.SerializeField]
    private string _text;

    public string Text => _text;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/NodeBased2.png)

```csharp
using OlegHcp.NodeBased;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(OlegHcp) + "/Graph/" + nameof(ExampleGraph), fileName = nameof(ExampleGraph))]
public class ExampleGraph : Graph<ExampleNode>
{

}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/NodeBased3.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/NodeBased4.png)

```csharp
using OlegHcp.NodeBased;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private ExampleGraph _graph;

    private void DoSomething()
    {
        ExampleNode node = _graph.RootNode;

        foreach (TransitionInfo<ExampleNode> transition in node)
        {
            if (!transition.IsExit)
            {
                Debug.Log(transition.NextNode.Text);
            }
        }
    }
}
```

### Custom Node View

```csharp
using OlegHcp;
using OlegHcpEditor.NodeBased;
using UnityEditor;
using UnityEngine;

[CustomNodeDrawer(typeof(ExampleNode))]
public class ExampleNodeDrawer : NodeDrawer
{
    protected override float GetHeight(SerializedProperty property)
    {
        return base.GetHeight(property) * 2f;
    }

    protected override Color GetHeaderColor(bool rootNode)
    {
        return rootNode ? Colours.Sky : Colours.Lime;
    }

    protected override void OnGui(SerializedProperty property, float width)
    {
        EditorGUIUtility.labelWidth = width * 0.5f;
        EditorGUILayout.LabelField("Overridden", "Qwerty");
        GUILayout.Button("Button");
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/NodeBased5.png)
