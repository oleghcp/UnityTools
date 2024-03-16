## MonoSingleton

Abstract generic class derived from MonoBehaviour.  
Has lazy initialization with creating instance in runtime.

```csharp
public class MySingleton : MonoSingleton<MySingleton>
{
    // Used instead of Awake()
    protected override void Construct()
    {

    }

    // Used instead of OnDestroy()
    protected override void Destruct()
    {

    }
	
    public void DoSome()
    {
        // Do something
    }
}
```

```csharp
public class MyClass
{
    public void MyMethod()
    {
        MySingleton.I.DoSome();
    }
}
```

## CreateInstanceAttribute

Alternative instancing

```csharp
using OlegHcp.SingleScripts;
using UnityEngine;

[Creating]
public class MySingleton : MonoSingleton<MySingleton>
{
    // Called on Awake
    protected override void Construct()
    {

    }

    // Called on OnDestroy
    protected override void Destruct()
    {

    }

    private class CreatingAttribute : CreateInstanceAttribute
    {
        public override void Create()
        {
            var aaset = Resources.Load(/*prefab path*/);
            UnityEngine.Object.Instantiate(asset);
        }
    }
}
```
