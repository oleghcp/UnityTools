### MonoTimer

```csharp
using OlegHcp;
using OlegHcp.Timers;

public class MyClass
{
    private ITimer _timer = new MonoTimer();

    public void Start(float time)
    {
        _timer.Elapsed_Event += OnElapsed;
        _timer.Start(time);
    }

    private void OnElapsed(ITimer timer)
    {
        // Do something
    }
}
```