### CertainTypesAttribute

Allows to assign objects only of the specified types in inspector

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [CertainTypes(typeof(GameObject), typeof(MonoBehaviour))]
    private UnityEngine.Object _ref;
}
```

### ClampDiapasonAttribute

```csharp
using OlegHcp.Inspector;
using OlegHcp.NumericEntities;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [ClampDiapason(0f, 1f)]
    private Diapason _diapason;
}
```

### DisableEditingAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [DisableEditing]
    private string _text = "Qwerty";
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/DisableEditing.png)

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
    [SerializeField]
    [DrawFlags(typeof(FlagExample))]
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

### DrawObjectFieldsAttribute

```csharp
using UnityEngine;

public class TestSo : ScriptableObject
{
    [SerializeField]
    private int _foo;
    [SerializeField]
    private string _bar;
}
```

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [DrawObjectFields(true)]
    private TestSo _ref;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/DrawObjectFields.png)

### EnumMenuAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public enum MyEnum
{
    A, B, C, D, E, F,
}

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [EnumMenu]
    private MyEnum _foo;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/EnumMenu.png)

### FolderRequiredAttribute

Allows to assign only folder assets in inspector

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [FolderRequired]
    private UnityEngine.Object _folderAsset;
}
```

### IdentifierAttribute

Generates unique string id

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [Identifier(true)]
    private string _id;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/Identifier.png)
