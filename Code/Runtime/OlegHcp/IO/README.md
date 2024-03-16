## BinaryFileUtility

```csharp
using OlegHcp.IO;

public class MyClass
{
    public void DoSomething(int[] nums)
    {
        string path = "D:\\FileName.dat";

        BinaryFileUtility.Save(path, nums);

        int[] nums2 = BinaryFileUtility.Load<int[]>(path);
    }
}
```
