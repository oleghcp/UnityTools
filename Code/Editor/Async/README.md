## OlegHcpEditor.Async

### EditorTaskSystem

Same as runtime `TaskSystem` just for editor scripts.  
Uses `com.unity.editorcoroutines` package.

```csharp
public static class EditorTaskSystem
{
    public static TaskInfo StartAsync(IEnumerator routine, in CancellationToken token = default);
    public static TaskInfo RunDelayed(float time, Action run, in CancellationToken token = default);
    public static TaskInfo RunAfterFrames(int frames, Action run, in CancellationToken token = default);
    public static TaskInfo RunByCondition(Func<bool> condition, Action run, in CancellationToken token = default);
    public static TaskInfo Repeat(Func<bool> condition, Action run, in CancellationToken token = default);
}
```
