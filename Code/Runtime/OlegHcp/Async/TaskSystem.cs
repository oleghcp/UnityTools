using System;
using System.Collections;
using System.Threading;
using OlegHcp.Engine;
using OlegHcp.IdGenerating;

namespace OlegHcp.Async
{
    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class TaskSystem
    {
        private static readonly LongIdGenerator _idProvider = new LongIdGenerator();
        private static TaskDispatcher _globals;
        private static TaskDispatcher _locals;

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator routine, in CancellationToken token)
        {
            return GetGlobal().RunAsync(routine, _idProvider.GetNewId(), false, token);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine. Call of Stop() method is prohibited.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator routine, bool unstoppable = false)
        {
            return GetGlobal().RunAsync(routine, _idProvider.GetNewId(), unstoppable, default);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine (just for current scene).
        /// </summary>
        public static TaskInfo StartAsyncLocally(IEnumerator routine, in CancellationToken token)
        {
            return GetLocal().RunAsync(routine, _idProvider.GetNewId(), false, token);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine (just for current scene). Call of Stop() method is prohibited.
        /// </summary>
        public static TaskInfo StartAsyncLocally(IEnumerator routine, bool unstoppable = false)
        {
            return GetLocal().RunAsync(routine, _idProvider.GetNewId(), unstoppable, default);
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public static TaskInfo RunDelayed(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return GetGlobal().RunAsync(CoroutineUtility.GetRunDelayedRoutine(time, run, scaledTime), _idProvider.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function after delay (just for current scene).
        /// </summary>
        public static TaskInfo RunDelayedLocally(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return GetLocal().RunAsync(CoroutineUtility.GetRunDelayedRoutine(time, run, scaledTime), _idProvider.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public static TaskInfo RunAfterFrames(int frames, Action run, in CancellationToken token = default)
        {
            return GetGlobal().RunAsync(CoroutineUtility.GetRunAfterFramesRoutine(frames, run), _idProvider.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count (just for current scene).
        /// </summary>
        public static TaskInfo RunAfterFramesLocally(int frames, Action run, in CancellationToken token = default)
        {
            return GetLocal().RunAsync(CoroutineUtility.GetRunAfterFramesRoutine(frames, run), _idProvider.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunByCondition(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetGlobal().RunAsync(CoroutineUtility.GetRunByConditionRoutine(condition, run), _idProvider.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RunByConditionLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetLocal().RunAsync(CoroutineUtility.GetRunByConditionRoutine(condition, run), _idProvider.GetNewId(), false, token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo Repeat(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetGlobal().RunAsync(CoroutineUtility.GetRunWhileRoutine(condition, run), _idProvider.GetNewId(), false, token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RepeatLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetLocal().RunAsync(CoroutineUtility.GetRunWhileRoutine(condition, run), _idProvider.GetNewId(), false, token);
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
