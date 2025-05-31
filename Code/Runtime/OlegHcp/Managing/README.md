## ServiceLocator

### BaseServiceLocator

```csharp
using System;
using System.Collections.Generic;
using OlegHcp;
using OlegHcp.Managing;
using UnityObject = UnityEngine.Object;

public class ExampleContext : Dictionary<Type, Func<IService>>, IInitialContext
{
    public ExampleContext()
    {
        Add(typeof(IServiceA), UnityObject.FindObjectOfType<ServiceA>);
        Add(typeof(ServiceB), () => new ServiceB());
        Add(typeof(ServiceC), ComponentUtility.CreateInstance<ServiceC>);
    }

    public bool TryCreateInstance(Type serviceType, out IService service)
    {
        bool success = TryGetValue(serviceType, out var func);
        service = func?.Invoke();
        return success;
    }
}
```

```csharp
using OlegHcp.Managing;

public static class Services
{
    private static IServiceLocator _serviceLocator = new BaseServiceLocator(new ExampleContext());

    public static IServiceLocator Locator => _serviceLocator;
}
```

### SimpleServiceLocator

```csharp
using OlegHcp.Managing;

public static class Services
{
    private static DirtyServiceLocator _serviceLocator = new DirtyServiceLocator();

    public static DirtyServiceLocator Locator => _serviceLocator;
}
```

```csharp
using OlegHcp;
using UnityEngine;

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
        // Stored services can be IDisposable
        Services.Locator.Remove<IServiceA>();
        Services.Locator.Remove<ServiceB>();
        Services.Locator.Remove<ServiceC>();
    }
}
```

### Usage

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

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/ServiceLocator.png)
