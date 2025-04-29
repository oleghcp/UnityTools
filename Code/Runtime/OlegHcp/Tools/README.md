## ConvertUtility

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    private void Start()
    {
        long num = 0xFFFF;

        // Convert long number to string with custom radix
        string hexString = ConvertUtility.DecimalToStringWithCustomRadix(num, 16);
        // Result: string "FFFF"

        //parse string to long with the same radix
        long num2 = ConvertUtility.ParseStringCustomRadixToDecimal(hexString, 16);
    }
}
```

## Helper

```csharp
public static class Helper
{
    public static void Swap<T>(ref T a, ref T b);
    public static object GetDefaultValue(Type type);
    public static object CloneObject(object source);
}
```

## SceneTool

```
public static class SceneTool
{
    public static IReadOnlyList<GameObject> GetDontDestroyOnLoadObjects();
    public static void GetDontDestroyOnLoadObjects(List<GameObject> buffer);
}
```