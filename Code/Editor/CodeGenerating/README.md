## OlegHcpEditor.CodeGenerating

### EnumGenerator

```csharp
public static class EnumGenerator
{
    public static string Generate(string nameSpace, string enumName, IEnumerable<string> enumValues, EnumType enumType = EnumType.Int);
    public static string Generate(string nameSpace, string enumName, IEnumerable<(string name, int intValue)> enumValues, EnumType enumType = EnumType.Int);
}
```

```csharp
public enum EnumType
{
    SByte,
    Byte,
    Short,
    UShort,
    Int,
    UInt,
    Long,
    ULong,
}
```

### GeneratingTools

```csharp
public static class GeneratingTools
{
    public static readonly string Tab = "    ";

    public static void CreateCsFile(string text, string rootFolder, string className, string nameSpace, bool refreshAssets = true);
    public static void GenerateBanner(StringBuilder builder);
}
```
