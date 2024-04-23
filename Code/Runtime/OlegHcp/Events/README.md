## SignalBus

```csharp
using OlegHcp.Events;

public static class EventBus
{
    private static SignalBus _signalBus = new SignalBus();
    public static SignalBus Current => _signalBus;
}
```

```csharp
using OlegHcp.Events;

public struct ExampleSignal : ISignal
{
    public int Value;
}
```

```csharp
public class ExampleEventListener1 : IDisposable
{
    public ExampleEventListener1()
    {
        // Subscribe with priority 1
        EventBus.Current.Subscribe<ExampleSignal>(OnGotEvent, 1);
    }

    private void OnGotEvent(ExampleSignal signal)
    {
        // Do something with signal.Value
    }

    public void Dispose()
    {
        EventBus.Current.Unsubscribe<ExampleSignal>(OnGotEvent);
    }
}

public class ExampleEventListener2 : IDisposable
{
    public ExampleEventListener2()
    {
        // Subscribe with priority 2 which is lower than 1 and will executed after
        EventBus.Current.Subscribe<ExampleSignal>(OnGotEvent, 2);
    }

    private void OnGotEvent(ExampleSignal signal)
    {
        // Do something with signal.Value
    }

    public void Dispose()
    {
        EventBus.Current.Unsubscribe<ExampleSignal>(OnGotEvent);
    }
}
```

```csharp
using OlegHcp.Events;
using UnityEngine;

public class ExampleEventOwner : MonoBehaviour
{
    private BusEvent _event;

    private void Awake()
    {
        _event = EventBus.Current.RegisterEventOwner<ExampleSignal>(this);
    }

    private void Update()
    {
        if (/*some condition*/)
        {
            _event.Invoke(new ExampleSignal { Value = /*some value*/ });
        }
    }

    private void OnDestroy()
    {
        _event.UnregisterOwner();
    }
}
```
