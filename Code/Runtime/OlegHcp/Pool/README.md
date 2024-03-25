## ObjectPool

```csharp
using OlegHcp.Pool;
using UnityEngine;

public class ExampleObject : MonoBehaviour, IPoolable
{
    // On get object from pool
    public void Reinit()
    {
        gameObject.SetActive(true);
    }

    // On release object to pool
    public void CleanUp()
    {
        gameObject.SetActive(false);
    }
}
```

Stored objects can be IDisposable.

```csharp
using OlegHcp.Pool;
using UnityEngine;

public static class ExamplePool
{
    private static ObjectPool<ExampleObject> _pool = new ObjectPool<ExampleObject>(CreateObject);

    public static ExampleObject GetObject()
    {
        return _pool.Get();
    }

    public static void ReleaseObject(ExampleObject obj)
    {
        _pool.Release(obj);
    }

    public static void OnSceneChanged()
    {
        _pool.Clear();
    }

    private static ExampleObject CreateObject()
    {
        return new GameObject().AddComponent<ExampleObject>();
    }
}
```

### Custom implenetation of pool storage

```csharp
using OlegHcp.Pool;

public class Storage<T> : IPoolStorage<T> where T : class, IPoolable
{
    public int Count => /* Count */;

    public bool TryAdd(T value)
    {
        // Add element
    }

    public bool TryGet(out T value)
    {
        // Remove and return element
    }

    public void Clear()
    {
        // Clear storage
    }
}
```
