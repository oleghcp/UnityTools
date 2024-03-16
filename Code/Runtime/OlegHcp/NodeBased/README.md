## Node Based

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/NodeBased1.png)

```csharp
using System;
using OlegHcp.NodeBased;
using UnityEngine;

namespace Assets.Code
{
    [Serializable]
    public class DialogueNode : Node<DialogueNode>
    {
        [SerializeField]
        private string _text;

        public string Text => _text;
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/NodeBased2.png)

```csharp
using OlegHcp.NodeBased;
using UnityEngine;

namespace Assets.Code
{
    [CreateAssetMenu(menuName = nameof(OlegHcp) + "/Graph/" + nameof(DialogueGraph), fileName = nameof(DialogueGraph))]
    public class DialogueGraph : Graph<DialogueNode>
    {

    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/NodeBased3.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/NodeBased4.png)