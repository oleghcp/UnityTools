## ArrayExtensions

```csharp
using System;
using OlegHcp;
using OlegHcp.CSharp;

public class Example
{
    private void DoSomething()
    {
        int[] array = new int[100];

        array.Fill(index => RandomNumberGenerator.Default.Next(0, 100));
        array.Sort();

        int index = array.IndexOf(50);

        if (index >= 0)
        {
            // Do something
        }

        array.ForEach(item => UnityEngine.Debug.Log(item));

        ArraySegment<int> segment = array.Slice(10, 50);

        array.Clear();
    }
}
```
