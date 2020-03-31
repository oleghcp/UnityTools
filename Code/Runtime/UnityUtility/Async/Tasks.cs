using System;
using System.Collections;

namespace UnityUtility.Async
{
    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class Tasks
    {
        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator run)
        {
            return AsyncStuffPool.GetExecutor().RunAsync(run);
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public static TaskInfo RunDelayed(float time, Action run, bool scaledTime = true)
        {
            return AsyncStuffPool.GetExecutor().RunAsync(Script.RunDelayedRoutine(time, run, scaledTime));
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunByCondition(Func<bool> condition, Action run)
        {
            return AsyncStuffPool.GetExecutor().RunAsync(Script.RunByConditionRoutine(condition, run));
        }

        /// <summary>
        /// Runs a referenced function on the next frame.
        /// </summary>
        public static TaskInfo RunNextFrame(Action run)
        {
            return AsyncStuffPool.GetExecutor().RunAsync(Script.RunAfterFramesRoutine(1, run));
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public static TaskInfo RunAfterFrames(int frames, Action run)
        {
            return AsyncStuffPool.GetExecutor().RunAsync(Script.RunAfterFramesRoutine(frames, run));
        }

        /// <summary>
        /// Runs a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunWhile(Func<bool> condition, Action run)
        {
            return AsyncStuffPool.GetExecutor().RunAsync(Script.RunWhileRoutine(condition, run));
        }
    }
}
