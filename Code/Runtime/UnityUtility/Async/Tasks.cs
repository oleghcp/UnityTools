using System;
using System.Collections;
using UnityEngine;

namespace UnityUtility.Async
{
    public interface ITaskFactory : IDisposable
    {
        long GetNewId();
        void Release(ITask runner);
        ITask GetRunner();
    }

    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class Tasks
    {
        private static ITaskFactory s_factory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetUp()
        {
            s_factory = new SimpleRoutineRunnerFactory();
        }

        public static void OverrideFactory(ITaskFactory newFactory)
        {
            s_factory.Dispose();
            s_factory = newFactory;
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator run)
        {
            return s_factory.GetRunner().RunAsync(run);
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public static TaskInfo RunDelayed(float time, Action run, bool scaledTime = true)
        {
            return s_factory.GetRunner().RunAsync(Script.RunDelayedRoutine(time, run, scaledTime));
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunByCondition(Func<bool> condition, Action run)
        {
            return s_factory.GetRunner().RunAsync(Script.RunByConditionRoutine(condition, run));
        }

        /// <summary>
        /// Runs a referenced function on the next frame.
        /// </summary>
        public static TaskInfo RunNextFrame(Action run)
        {
            return s_factory.GetRunner().RunAsync(Script.RunAfterFramesRoutine(1, run));
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public static TaskInfo RunAfterFrames(int frames, Action run)
        {
            return s_factory.GetRunner().RunAsync(Script.RunAfterFramesRoutine(frames, run));
        }

        /// <summary>
        /// Runs a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunWhile(Func<bool> condition, Action run)
        {
            return s_factory.GetRunner().RunAsync(Script.RunWhileRoutine(condition, run));
        }
    }
}
