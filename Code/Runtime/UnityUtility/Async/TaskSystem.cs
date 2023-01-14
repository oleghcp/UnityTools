using System;
using System.Collections;
using System.Threading;
using UnityUtility.Engine;
using UnityUtility.IdGenerating;

namespace UnityUtility.Async
{
    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class TaskSystem
    {
        private static TaskDispatcher _globals;
        private static TaskDispatcher _locals;

        internal static LongIdGenerator IdProvider { get; } = new LongIdGenerator();

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator run, in CancellationToken token = default)
        {
            return GetGlobal().GetRunner().RunAsync(run, token);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine (just for current scene).
        /// </summary>
        public static TaskInfo StartAsyncLocally(IEnumerator run, in CancellationToken token = default)
        {
            return GetLocal().GetRunner().RunAsync(run, token);
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public static TaskInfo RunDelayed(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return GetGlobal().GetRunner().RunAsync(CoroutineUtility.GetRunDelayedRoutine(time, run, scaledTime), token);
        }

        /// <summary>
        /// Runs a referenced function after delay (just for current scene).
        /// </summary>
        public static TaskInfo RunDelayedLocally(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return GetLocal().GetRunner().RunAsync(CoroutineUtility.GetRunDelayedRoutine(time, run, scaledTime), token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public static TaskInfo RunAfterFrames(int frames, Action run, in CancellationToken token = default)
        {
            return GetGlobal().GetRunner().RunAsync(CoroutineUtility.GetRunAfterFramesRoutine(frames, run), token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count (just for current scene).
        /// </summary>
        public static TaskInfo RunAfterFramesLocally(int frames, Action run, in CancellationToken token = default)
        {
            return GetLocal().GetRunner().RunAsync(CoroutineUtility.GetRunAfterFramesRoutine(frames, run), token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunByCondition(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetGlobal().GetRunner().RunAsync(CoroutineUtility.GetRunByConditionRoutine(condition, run), token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RunByConditionLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetLocal().GetRunner().RunAsync(CoroutineUtility.GetRunByConditionRoutine(condition, run), token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo Repeat(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetGlobal().GetRunner().RunAsync(CoroutineUtility.GetRunWhileRoutine(condition, run), token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RepeatLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetLocal().GetRunner().RunAsync(CoroutineUtility.GetRunWhileRoutine(condition, run), token);
        }

        private static TaskDispatcher GetGlobal()
        {
            if (_globals == null)
            {
                _globals = ComponentUtility.CreateInstance<TaskDispatcher>();
                _globals.Immortalize();
            }

            return _globals;
        }

        private static TaskDispatcher GetLocal()
        {
            if (_locals == null)
                _locals = ComponentUtility.CreateInstance<TaskDispatcher>();

            return _locals;
        }
    }
}
