## ArrayExtensions

```csharp
using System;
using OlegHcp;
using OlegHcp.CSharp;
using UnityEngine;

public class Example
{
    private void DoSomething()
    {
        Vector3[] array = new Vector3[100];
        IRng rng = RandomNumberGenerator.Default;

        array.Fill(index => new Vector3(rng.Next(100f), rng.Next(100f), rng.Next(100f)));
        array.Sort(item => item.magnitude);

        int index = array.IndexOf(item => item.magnitude > 50f);

        if (index >= 0)
        {
            // Do something
        }

        array.ForEach(item => Debug.Log(item));

        ArraySegment<Vector3> segment = array.Slice(10, 50);

        array.Clear();
    }
}
```
