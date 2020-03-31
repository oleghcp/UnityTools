using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using UnityUtility.Collections;
using UnityUtility.IdGenerating;

namespace UnityUtility.Async
{
    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class Tasks
    {
        private class Data
        {
            private const string RUNNER_NAME = "Task";

            public readonly ObjectPool<RoutineRunner> RunnersPool;
            public readonly IdGenerator<long> IdProvider;

            public Data()
            {
                IdProvider = new LongIdGenerator();
                RunnersPool = new ObjectPool<RoutineRunner>(f_create);

                SceneManager.sceneUnloaded += _ => RunnersPool.Clear();
            }

            private RoutineRunner f_create()
            {
                return Script.CreateInstance<RoutineRunner>(RUNNER_NAME);
            }
        }

        private static readonly Data s_inst;

        static Tasks()
        {
            s_inst = new Data();
        }

        internal static long GetNewId()
        {
            return s_inst.IdProvider.GetNewId();
        }

        internal static void Return(RoutineRunner runner)
        {
            s_inst.RunnersPool.Release(runner);
        }

        // -- //

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator run)
        {
            return GetExecutor().RunAsync(run);
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public static TaskInfo RunDelayed(float time, Action run, bool scaledTime = true)
        {
            return GetExecutor().RunAsync(Script.RunDelayedRoutine(time, run, scaledTime));
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunByCondition(Func<bool> condition, Action run)
        {
            return GetExecutor().RunAsync(Script.RunByConditionRoutine(condition, run));
        }

        /// <summary>
        /// Runs a referenced function on the next frame.
        /// </summary>
        public static TaskInfo RunNextFrame(Action run)
        {
            return GetExecutor().RunAsync(Script.RunAfterFramesRoutine(1, run));
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public static TaskInfo RunAfterFrames(int frames, Action run)
        {
            return GetExecutor().RunAsync(Script.RunAfterFramesRoutine(frames, run));
        }

        /// <summary>
        /// Runs a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunWhile(Func<bool> condition, Action run)
        {
            return GetExecutor().RunAsync(Script.RunWhileRoutine(condition, run));
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static RoutineRunner GetExecutor()
        {
            return s_inst.RunnersPool.Get();
        }
    }
}
