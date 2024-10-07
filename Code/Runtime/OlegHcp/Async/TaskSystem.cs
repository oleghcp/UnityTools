using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using OlegHcp.CSharp.Collections;
using OlegHcp.Engine;
using OlegHcp.IdGenerating;
using UnityEngine;

namespace OlegHcp.Async
{
    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class TaskSystem
    {
        private static readonly LongIdGenerator _idProvider = new LongIdGenerator();
        private static readonly Dictionary<float, WaitForSeconds> _timeInstructions = new Dictionary<float, WaitForSeconds>();
        private static readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        private static readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private static TaskDispatcher _globals;
        private static TaskDispatcher _locals;

        public static WaitForEndOfFrame WaitForEndOfFrame => _waitForEndOfFrame;
        public static WaitForFixedUpdate WaitForFixedUpdate => _waitForFixedUpdate;
        internal static LongIdGenerator IdProvider => _idProvider;

        public static WaitForSeconds GetWaitInstruction(float seconds)
        {
            if (_timeInstructions.TryGetValue(seconds, out var instruction))
                return instruction;

            return _timeInstructions.Place(seconds, new WaitForSeconds(seconds));
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator routine, in CancellationToken token)
        {
            return GetGlobal().GetRunner().RunAsync(routine, false, token);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine. Call of Stop() method is prohibited.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator routine, bool unstoppable = false)
        {
            return GetGlobal().GetRunner().RunAsync(routine, unstoppable, default);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine (just for current scene).
        /// </summary>
        public static TaskInfo StartAsyncLocally(IEnumerator routine, in CancellationToken token)
        {
            return GetLocal().GetRunner().RunAsync(routine, false, token);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine (just for current scene). Call of Stop() method is prohibited.
        /// </summary>
        public static TaskInfo StartAsyncLocally(IEnumerator routine, bool unstoppable = false)
        {
            return GetLocal().GetRunner().RunAsync(routine, unstoppable, default);
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public static TaskInfo RunDelayed(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return GetGlobal().GetRunner().RunAsync(CoroutineUtility.GetRunDelayedRoutine(time, run, scaledTime), false, token);
        }

        /// <summary>
        /// Runs a referenced function after delay (just for current scene).
        /// </summary>
        public static TaskInfo RunDelayedLocally(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return GetLocal().GetRunner().RunAsync(CoroutineUtility.GetRunDelayedRoutine(time, run, scaledTime), false, token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public static TaskInfo RunAfterFrames(int frames, Action run, in CancellationToken token = default)
        {
            return GetGlobal().GetRunner().RunAsync(CoroutineUtility.GetRunAfterFramesRoutine(frames, run), false, token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count (just for current scene).
        /// </summary>
        public static TaskInfo RunAfterFramesLocally(int frames, Action run, in CancellationToken token = default)
        {
            return GetLocal().GetRunner().RunAsync(CoroutineUtility.GetRunAfterFramesRoutine(frames, run), false, token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunByCondition(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetGlobal().GetRunner().RunAsync(CoroutineUtility.GetRunByConditionRoutine(condition, run), false, token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RunByConditionLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetLocal().GetRunner().RunAsync(CoroutineUtility.GetRunByConditionRoutine(condition, run), false, token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo Repeat(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetGlobal().GetRunner().RunAsync(CoroutineUtility.GetRunWhileRoutine(condition, run), false, token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RepeatLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return GetLocal().GetRunner().RunAsync(CoroutineUtility.GetRunWhileRoutine(condition, run), false, token);
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
