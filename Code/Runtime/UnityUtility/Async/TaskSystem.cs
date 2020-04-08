using System;
using System.Collections;
using UnityEngine;

namespace UnityUtility.Async
{
    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class TaskSystem
    {
        public const string SYSTEM_NAME = "Async System (ext.)";

        private static TaskFactory s_factory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void f_setUp()
        {
            s_factory = new TaskFactory();
        }

        public static void RegisterStopper(ITaskStopper stopper)
        {
            s_factory.RegisterStopper(stopper);
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
