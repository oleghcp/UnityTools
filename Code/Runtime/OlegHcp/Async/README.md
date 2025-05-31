## Coroutine Running

```csharp
using System.Collections;
using OlegHcp.Async;
using UnityEngine;

public class MyClass
{
    private TaskInfo _task;

    public void Start()
    {
        // Base start option
        _task = TaskSystem.StartAsync(RunAsyncStuff());
        // Or use extension
        _task = RunAsyncStuff().StartAsyncLocally();

        // Complete callback
        _task.AddCompleteListener(onComplete);

        void onComplete(TaskResult resultInfo)
        {
            if (resultInfo.Successful)
                Debug.Log(resultInfo.Result);
        }
    }

    public void Stop()
    {
        _task.Stop();
    }

    private IEnumerator RunAsyncStuff()
    {
        while (/*condition*/)
        {
            // do something
            yield return null;
        }

        yield return /*result*/;
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/TaskSystem.png)

Also if you want to facilitate starting of coroutines you can create an extension class.  
Important: in order to consider the extensions in analyzing of stack trace the class should be named `RoutineExtensions`.

```
public static class RoutineExtensions
{
    public static TaskInfo StartAsync(this IEnumerator self, bool unstoppable = false)
    {
        return TaskSystem.StartAsync(self, unstoppable);
    }

    public static TaskInfo StartAsync(this IEnumerator self, in CancellationToken token)
    {
        return TaskSystem.StartAsync(self, token);
    }

    public static TaskInfo StartAsyncLocally(this IEnumerator self, bool unstoppable = false)
    {
        return TaskSystem.StartAsyncLocally(self, unstoppable);
    }

    public static TaskInfo StartAsyncLocally(this IEnumerator self, in CancellationToken token)
    {
        return TaskSystem.StartAsyncLocally(self, token);
    }
}
```

### Other Functions

```csharp
using OlegHcp.Async;
using UnityEngine;

public class MyClass
{
    public void Start1()
    {
        const float duration = 100f;
        float startTime = Time.time;
        TaskSystem.Repeat(() => Time.time - startTime < duration, perFrameFunction);

        void perFrameFunction()
        {
            // Do something
        }
    }

    public void Start2()
    {
        const float delay = 100f;
        float startTime = Time.time;
        TaskSystem.RunByCondition(() => Time.time - startTime > delay, dependentFunction);

        void dependentFunction()
        {
            // Do something
        }
    }
    
    public void Start3()
    {
        TaskSystem.RunDelayed(10f, perFrameFunction);

        void perFrameFunction()
        {
            // Do something
        }
    }
}
```
