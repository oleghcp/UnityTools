## Diapason

```csharp
using OlegHcp.NumericEntities;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private Diapason _range = (0f, 10f);

    private void Start()
    {
        Debug.Log(_range.Min);
        Debug.Log(_range.Max);
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/Diapason.png)
