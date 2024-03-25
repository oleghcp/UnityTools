## AlphanumComparer

```csharp
using System.Collections.Generic;
using OlegHcp.Strings;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    private void Start()
    {
        List<string> strings = new List<string>()
        {
            "10",
            "20",
            "2",
            "100",
            "3",
            "15",
        };

        strings.Sort();

        //Result: 10, 100, 15, 2, 20, 3

        strings.Sort(new AlphanumComparer());

        //Result: 2, 3, 10, 15, 20, 100
    }
}
```
