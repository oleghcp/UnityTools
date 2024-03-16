## MonoSingleton

Abstract generic class derived from `MonoBehaviour`.  
Has lazy initialization with creating instance in runtime.

```csharp
public class MySingleton : MonoSingleton<MySingleton>
{
    // Used instead of Awake()
    protected override void Construct() { }

    // Used instead of OnDestroy()
    protected override void Destruct()  { }

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

Alternative `MonoSingleton` instancing

```csharp
using OlegHcp.SingleScripts;
using UnityEngine;

[Creating]
public class MySingleton : MonoSingleton<MySingleton>
{
    // Called on Awake
    protected override void Construct() { }

    // Called on OnDestroy
    protected override void Destruct() { }

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

## SingleBehaviour

Same as `MonoSingleton` but script have to be saved in scene before using. The instance property just looking for it in scene when called first time.

```csharp
public class MySingleton : SingleBehaviour<MySingleton>
{
   ...
```

## SingleUiBehaviour

Same as `SingleBehaviour` just has `rectTransform` property

## ScriptableSingleton

Abstract generic class derived from `ScriptableObject`.  
Has lazy initialization with creating instance in runtime.
Can be modified with `CreateInstanceAttribute`

## SingleScript

Like `SingleBehaviour` should be loaded manualy. The instance property just looking for it in scene when called first time.
