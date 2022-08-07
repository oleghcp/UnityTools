using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityUtility.IdGenerating;

namespace UnityUtility.Async
{
    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class TaskSystem
    {
        private static Data _data;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetUp()
        {
            LongIdGenerator idProvider = new LongIdGenerator();

            _data = new Data
            {
                Globals = new TaskFactory(idProvider, true, "Tasks"),
                Locals = new TaskFactory(idProvider, false, "LocalTasks")
            };
        }

        public static void RegisterStopper(ITaskStopper stopper)
        {
            if (_data.Stopper != null)
                throw new InvalidOperationException("Stoping object is already set.");

            _data.Stopper = stopper;

            _data.Globals.RegisterStopper(stopper);
            _data.Locals.RegisterStopper(stopper);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator run, in CancellationToken token = default)
        {
            return _data.Globals.GetRunner().RunAsync(run, token);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine (just for current scene).
        /// </summary>
        public static TaskInfo StartAsyncLocally(IEnumerator run, in CancellationToken token = default)
        {
            return _data.Locals.GetRunner().RunAsync(run, token);
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public static TaskInfo RunDelayed(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return _data.Globals.GetRunner().RunAsync(CoroutineUtility.RunDelayedRoutine(time, run, scaledTime), token);
        }

        /// <summary>
        /// Runs a referenced function after delay (just for current scene).
        /// </summary>
        public static TaskInfo RunDelayedLocally(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return _data.Locals.GetRunner().RunAsync(CoroutineUtility.RunDelayedRoutine(time, run, scaledTime), token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public static TaskInfo RunAfterFrames(int frames, Action run, in CancellationToken token = default)
        {
            return _data.Globals.GetRunner().RunAsync(CoroutineUtility.RunAfterFramesRoutine(frames, run), token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count (just for current scene).
        /// </summary>
        public static TaskInfo RunAfterFramesLocally(int frames, Action run, in CancellationToken token = default)
        {
            return _data.Locals.GetRunner().RunAsync(CoroutineUtility.RunAfterFramesRoutine(frames, run), token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunByCondition(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return _data.Globals.GetRunner().RunAsync(CoroutineUtility.RunByConditionRoutine(condition, run), token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RunByConditionLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return _data.Locals.GetRunner().RunAsync(CoroutineUtility.RunByConditionRoutine(condition, run), token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo Repeat(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return _data.Globals.GetRunner().RunAsync(CoroutineUtility.RunWhileRoutine(condition, run), token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RepeatLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return _data.Locals.GetRunner().RunAsync(CoroutineUtility.RunWhileRoutine(condition, run), token);
        }

        private class Data
        {
            public TaskFactory Globals;
            public TaskFactory Locals;
            public ITaskStopper Stopper;
        }
    }
}
