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

### Cached Yield Instructions

```csharp
using OlegHcp.Async;
using UnityEngine;
public class ExampleClass : MonoBehaviour
{
    // Cached yield instructions
    private IEnumerator GetRoutine()
    {
        yield return TaskSystem.WaitForEndOfFrame;
        yield return TaskSystem.WaitForFixedUpdate;
        yield return TaskSystem.GetWaitInstruction(10f);
    }
}
```
