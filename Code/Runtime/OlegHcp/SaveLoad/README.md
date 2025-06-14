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

public class Example : MonoBehaviour
{
    [SaveLoadField]
    private int _field1;

    // With optional default value
    [SaveLoadField]
    private float _field2 = 3.14f;

    // With custom key
    [SaveLoadField("1efb1c1b6a15cd944bb23d80d6c9d2a1")]
    private string _field3 = "DefaultValue";

    private void Start()
    {
        SaveSystem.LoadAndInitCurrent("Save1");

        // Add the object to the saver with initializing fields if they were saved, otherwise the fields have default values
        SaveSystem.SaveProvider.RegMember(this);
   
        Debug.Log(_field1);
        Debug.Log(_field2);
        Debug.Log(_field3);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _field1 = int.MaxValue;
            _field2 = float.MaxValue;
            _field3 = "Hello World!";
            SaveSystem.SaveData("Save1");
        }
    }

    private void OnDestroy()
    {
        SaveSystem.SaveProvider.UnregMember(this);
    }
}
```
