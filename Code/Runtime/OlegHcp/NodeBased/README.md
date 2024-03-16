## Node Based

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/NodeBased1.png)

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

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/NodeBased2.png)

```csharp
using OlegHcp.NodeBased;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(OlegHcp) + "/Graph/" + nameof(ExampleGraph), fileName = nameof(ExampleGraph))]
public class ExampleGraph : Graph<ExampleNode>
{

}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/NodeBased3.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/NodeBased4.png)

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

        foreach (TransitionInfo<ExampleNode> item in node)
        {
            if (!item.IsExit)
            {
                Debug.Log(item.NextNode.Text);
            }
        }
    }
}
```

### Custom Node View

```csharp
using OlegHcpEditor.NodeBased;
using UnityEditor;

[CustomNodeDrawer(typeof(ExampleNode))]
public class ExampleNodeDrawer : NodeDrawer
{
    protected override void OnGui(SerializedProperty property, float width)
    {
        EditorGUIUtility.labelWidth = width * 0.5f;
        EditorGUILayout.LabelField("Overridden", "Qwerty");
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/NodeBased5.png)
