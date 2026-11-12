## Node Based

![](https://drive.google.com/uc?export=view&id=1qotS_-fVk9UksH6Zu1nWrrSHa5CRcCaI)

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

![](https://drive.google.com/uc?export=view&id=1_bkxgA3RcTRcUB2SBokIlRqYhjvThcLN)

```csharp
using OlegHcp.NodeBased;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(OlegHcp) + "/Graph/" + nameof(ExampleGraph), fileName = nameof(ExampleGraph))]
public class ExampleGraph : Graph<ExampleNode>
{

}
```

![](https://drive.google.com/uc?export=view&id=1HbeqMMy0Cb5TOYBXZtzspGYo0-FePWt8)
![](https://drive.google.com/uc?export=view&id=1UG6aD-ImkRKPY_C_5uy3EWdrmNCRn8Wu)

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
            if (!transition.Exists)
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
    protected override float GetHeight(SerializedProperty node)
    {
        return base.GetHeight(node) * 2f + EditorGUIUtility.standardVerticalSpacing;
    }

    protected override Color GetHeaderColor(bool rootNode)
    {
        return rootNode ? Colours.Sky : Colours.Lime;
    }

    protected override void OnGui(SerializedProperty node, float width)
    {
        EditorGUILayout.LabelField("Overridden", "Qwerty");
        GUILayout.Button("Button");
    }
}
```

![](https://drive.google.com/uc?export=view&id=1mBvrFKr9pEvo7kNjTKhrenHSI7WyM8UX)
