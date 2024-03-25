## ServiceLocator

```csharp
using OlegHcp.Managing;

public static class Locator
{
    private static DirtyServiceLocator _serviceLocator = new DirtyServiceLocator();

    public static ServiceLocator Current => _serviceLocator;

    public static void Clear()
    {
        _serviceLocator.RemoveAll();
    }
}
```

```csharp
using OlegHcp;
using UnityEngine;

namespace Assets.PrivateStuff.Testing
{
    [DefaultExecutionOrder(-100)]
    public class ExampleSceneConstructor : MonoBehaviour
    {
        [SerializeField]
        private ServiceA _serviceA;

        private void Awake()
        {
            Locator.Current.Add<IServiceA>(() => _serviceA);
            Locator.Current.Add(() => new ServiceB());
            Locator.Current.Add(() => ComponentUtility.CreateInstance<ServiceC>());
        }

        private void OnDestroy()
        {
            Locator.Clear();
        }
    }
}
```

```csharp
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    private void Start()
    {
        IServiceA s1 = Locator.Current.Get<IServiceA>();
        ServiceB s2 = Locator.Current.Get<ServiceB>();
        ServiceC s3 = Locator.Current.Get<ServiceC>();

        // Do something
    }
}
```
