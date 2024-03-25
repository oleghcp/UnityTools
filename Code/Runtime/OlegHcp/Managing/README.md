## ServiceLocator

```csharp
using OlegHcp.Managing;

public static class Services
{
    private static DirtyServiceLocator _serviceLocator = new DirtyServiceLocator();

    public static DirtyServiceLocator Locator => _serviceLocator;
}
```

Stored services can be IDisposable.

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
            Services.Locator.AddContext<IServiceA>(() => _serviceA);
            Services.Locator.AddContext(new ServiceB());
            Services.Locator.AddContext(ComponentUtility.CreateInstance<ServiceC>);
        }

        private void OnDestroy()
        {
            Services.Locator.Remove<IServiceA>();
            Services.Locator.Remove<ServiceB>();
            Services.Locator.Remove<ServiceC>();
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
        IServiceA s1 = Services.Locator.Get<IServiceA>();
        ServiceB s2 = Services.Locator.Get<ServiceB>();
        ServiceC s3 = Services.Locator.Get<ServiceC>();

        // Do something
    }
}
```
