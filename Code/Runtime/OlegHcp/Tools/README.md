## Helper

```csharp
public static class Helper
{
    public static void Swap<T>(ref T a, ref T b);
    public static int GetHashCode(int hc0, int hc1);
    public static int GetHashCode(int hc0, int hc1, int hc2);
    public static int GetHashCode(int hc0, int hc1, int hc2, int hc3);
    public static object GetDefaultValue(Type type);
    public static object CloneObject(object source);
}
```