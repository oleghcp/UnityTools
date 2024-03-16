## MonoSingleton<T>

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