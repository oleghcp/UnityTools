#if EDITOR_COROUTINES
using System;
using System.Collections;
using System.Threading;
using OlegHcp.Async;
using Unity.EditorCoroutines.Editor;

namespace OlegHcpEditor.Async
{
    public static class EditorTaskSystem
    {
        public static TaskInfo StartAsync(IEnumerator routine, in CancellationToken token = default)
        {
            return RunAsync(routine, GetNewId(), false, token);
        }

        public static TaskInfo RunDelayed(float time, Action run, in CancellationToken token = default)
        {
            return RunAsync(CoroutineUtility.GetRunDelayedRoutine(time, run), GetNewId(), false, token);
        }

        public static TaskInfo RunAfterFrames(int frames, Action run, in CancellationToken token = default)
        {
            return RunAsync(CoroutineUtility.GetRunAfterFramesRoutine(frames, run), GetNewId(), false, token);
        }

        public static TaskInfo RunByCondition(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return RunAsync(CoroutineUtility.GetRunByConditionRoutine(condition, run), GetNewId(), false, token);
        }

        public static TaskInfo Repeat(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return RunAsync(CoroutineUtility.GetRunWhileRoutine(condition, run), GetNewId(), false, token);
        }

        private static TaskInfo RunAsync(IEnumerator routine, long id, bool unstoppable, in CancellationToken token)
        {
            RoutineIterator iterator = new RoutineIterator(routine, id, unstoppable, token);
            TaskInfo task = new TaskInfo(iterator);
            EditorCoroutineUtility.StartCoroutineOwnerless(iterator);
            return task;
        }

        private static long GetNewId()
        {
            return DateTime.Now.Ticks;
        }
    }
}
#endif
