﻿## SignalBus

```csharp
using OlegHcp.Events;

public static class Events
{
    private static SignalBus _signalBus = new SignalBus();
    public static SignalBus Bus => _signalBus;
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
        Events.Bus.Subscribe<ExampleSignal>(OnGotEvent, 1);
    }

    private void OnGotEvent(ExampleSignal signal)
    {
        // Do something with signal.Value
    }

    public void Dispose()
    {
        Events.Bus.Unsubscribe<ExampleSignal>(OnGotEvent);
    }
}

public class ExampleEventListener2 : IDisposable
{
    public ExampleEventListener2()
    {
        // Subscribe with priority 2 which is lower than 1 and will executed after
        Events.Bus.Subscribe<ExampleSignal>(OnGotEvent, 2);
    }

    private void OnGotEvent(ExampleSignal signal)
    {
        // Do something with signal.Value
    }

    public void Dispose()
    {
        Events.Bus.Unsubscribe<ExampleSignal>(OnGotEvent);
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
        _event = Events.Bus.LockEvent<ExampleSignal>();
    }

    private void Update()
    {
        if (/*some condition*/)
        {
            //proper invocation
            _event.Invoke(new ExampleSignal { Value = /*some value*/ });
            
            //this invocation causes exception
            Events.Bus.Invoke(new ExampleSignal { Value = /*some value*/ });
        }
    }

    private void OnDestroy()
    {
        _event.Unlock();
    }
}
```
