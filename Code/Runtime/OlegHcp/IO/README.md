## BinaryFileUtility

```csharp
using OlegHcp.IO;

public class MyClass
{
    public void DoSomething(int[] nums)
    {
        string path = "D:/FileName.dat";

        BinaryFileUtility.Save(path, nums);

        int[] nums2 = BinaryFileUtility.Load<int[]>(path);
    }
}
```

## PathUtility

```csharp
using OlegHcp.IO;
using UnityEngine;

public class MyClass
{
    public void DoSomething()
    {
        string parent = PathUtility.GetParentPath("D:/Foo/Bar/Name", 2);

        //Result: "D:/Foo"

        string skipped = PathUtility.SkipRootSteps("D:/Foo/Bar/Name", 2);

        //Result: "Bar/Name"
    }
}
```
