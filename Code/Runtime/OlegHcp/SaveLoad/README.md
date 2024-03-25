## SaveProvider

```csharp
public static class SaveSystem
{
    private static SaveProvider _saveProvider = new SaveProvider(new BinarySaver());

    public static SaveProvider SaveProvider => _saveProvider;

    public static void LoadAndInitCurrent(string version)
    {
        _saveProvider.Load(version, true);
    }

    public static void LoadWithReloading(string version)
    {
        // Unload all stuff before, for example unload all scenes and load temporary scene

        _saveProvider.Load(version);

        // Then load all stuff back
    }

    public static void SaveData(string version)
    {
        _saveProvider.Save(version);
    }
}
```

```csharp
using System;
using OlegHcp.SaveLoad;

public class Example : IDisposable
{
    [SaveLoadField]
    private int _field1;

    // With optional default value
    [SaveLoadField(3.14f)]
    private float _field2;

    [SaveLoadField("DefaultValue")]
    private string _field3;

    public Example()
    {
        // Add the object to the saver with initializing fields by saved values
        SaveSystem.SaveProvider.RegMember(this);
    }

    public void Dispose()
    {
        SaveSystem.SaveProvider.UnregMember(this);
    }
}
```
