### CertainTypesAttribute

Allows to assign objects only of the specified types in inspector

```csharp
public class MyClass : MonoBehaviour
{
    [SerializeField]
    [CertainTypes(typeof(GameObject), typeof(MonoBehaviour))]
    private UnityEngine.Object _ref;
}
```

### DrawFlagsAttribute

Is used for IntMask and BitList fields.

```csharp
public enum FlagExample
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    E = 4,
}
```

```csharp
using OlegHcp;
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField, DrawFlags(typeof(FlagExample))]
    private IntMask _mask;

    public void DoSomething(FlagExample flag)
    {
        if (_mask[(int)flag])
        {
            // Do something
        }
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/IntMask.png)
