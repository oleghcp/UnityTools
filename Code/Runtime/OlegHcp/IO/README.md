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

## DirectoryUtility

```csharp
public static class DirectoryUtility
{
    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, bool overwrite);
}
```