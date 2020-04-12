﻿using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityUtility.IdGenerating;

namespace UnityUtility.Async
{
    internal interface IAsyncSettings
    {
        bool CanBeStopped { get; }
        bool CanBeStoppedGlobally { get; }
    }

    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class TaskSystem
    {
        public const string SYSTEM_NAME = "Async System (ext.)";

        private static TaskFactory s_globals;
        private static TaskFactory s_locals;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void f_setUp()
        {
            IAsyncSettings settings = GetSettings();
            LongIdGenerator idProvider = new LongIdGenerator();

            s_globals = new TaskFactory(settings, idProvider, true);
            s_locals = new TaskFactory(settings, idProvider, false);
        }

        public static void RegisterStopper(ITaskStopper stopper)
        {
            s_globals.RegisterStopper(stopper);
            s_locals.RegisterStopper(stopper);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator run, in CancellationToken token = default)
        {
            return s_globals.GetRunner().RunAsync(run, token);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine (just for current scene).
        /// </summary>
        public static TaskInfo StartAsyncLocally(IEnumerator run, in CancellationToken token = default)
        {
            return s_locals.GetRunner().RunAsync(run, token);
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public static TaskInfo RunDelayed(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return s_globals.GetRunner().RunAsync(Script.RunDelayedRoutine(time, run, scaledTime), token);
        }

        /// <summary>
        /// Runs a referenced function after delay (just for current scene).
        /// </summary>
        public static TaskInfo RunDelayedLocally(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return s_locals.GetRunner().RunAsync(Script.RunDelayedRoutine(time, run, scaledTime), token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public static TaskInfo RunAfterFrames(int frames, Action run, in CancellationToken token = default)
        {
            return s_globals.GetRunner().RunAsync(Script.RunAfterFramesRoutine(frames, run), token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count (just for current scene).
        /// </summary>
        public static TaskInfo RunAfterFramesLocally(int frames, Action run, in CancellationToken token = default)
        {
            return s_locals.GetRunner().RunAsync(Script.RunAfterFramesRoutine(frames, run), token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunByCondition(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return s_globals.GetRunner().RunAsync(Script.RunByConditionRoutine(condition, run), token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RunByConditionLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return s_locals.GetRunner().RunAsync(Script.RunByConditionRoutine(condition, run), token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo Repeat(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return s_globals.GetRunner().RunAsync(Script.RunWhileRoutine(condition, run), token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RepeatLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return s_locals.GetRunner().RunAsync(Script.RunWhileRoutine(condition, run), token);
        }

        // -- //

        private static IAsyncSettings GetSettings()
        {
            AsyncSystemSettings settings = Resources.Load<AsyncSystemSettings>(nameof(AsyncSystemSettings));

            if (settings == null)
                return new DefaultSettings();

            return settings;
        }

        private class DefaultSettings : IAsyncSettings
        {
            bool IAsyncSettings.CanBeStopped => true;
            bool IAsyncSettings.CanBeStoppedGlobally => false;
        }
    }
}