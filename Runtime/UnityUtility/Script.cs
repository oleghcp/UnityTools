using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UU
{
    public struct Routine
    {
        internal Coroutine Coroutine;
        internal MonoBehaviour Executor;

        public MonoBehaviour Script
        {
            get { return Executor; }
        }

        public void Stop()
        {
            if (Coroutine != null && Executor != null)
                Executor.StopCoroutine(Coroutine);
        }
    }

    /// <summary>
    /// Inherits from MonoBehaviour and provides several subsidiary methods.
    /// </summary>
    public class Script : MonoBehaviour
    {
        /// <summary>
        /// Creates and returns an instance of GameObject with Component of T type.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateInstance<T>() where T : Component
        {            
            return CreateInstance<T>(typeof(T).Name);
        }

        /// <summary>
        /// Creates and returns an instance of GameObject with Component of T type.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        public static T CreateInstance<T>(string gameObjectName) where T : Component
        {
            GameObject go = new GameObject(gameObjectName);
            return go.AddComponent(typeof(T)) as T;
        }

        //--//

        /// <summary>
        /// Starts a coroutine.
        /// </summary>
        public Routine StartAsync(IEnumerator routine)
        {
            return new Routine
            {
                Coroutine = StartCoroutine(routine),
                Executor = this,
            };
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public Routine RunDelayed(float time, Action run, bool scaledTime = true)
        {
            return new Routine
            {
                Coroutine = StartCoroutine(RunDelayedRoutine(time, run, scaledTime)),
                Executor = this,
            };
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public Routine RunByCondition(Func<bool> condition, Action run)
        {
            return new Routine
            {
                Coroutine = StartCoroutine(RunByConditionRoutine(condition, run)),
                Executor = this,
            };
        }

        /// <summary>
        /// Runs a referenced function on the next frame.
        /// </summary>
        public Routine RunNextFrame(Action run)
        {
            return new Routine
            {
                Coroutine = StartCoroutine(RunAfterFramesRoutine(1, run)),
                Executor = this,
            };
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public Routine RunAfterFrames(int frames, Action run)
        {
            return new Routine
            {
                Coroutine = StartCoroutine(RunAfterFramesRoutine(frames, run)),
                Executor = this,
            };
        }

        /// <summary>
        /// Runs a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public Routine RunWhile(Func<bool> condition, Action run)
        {
            return new Routine
            {
                Coroutine = StartCoroutine(RunWhileRoutine(condition, run)),
                Executor = this,
            };
        }

        //--//

        internal static IEnumerator RunDelayedRoutine(float time, Action run, bool scaledTime)
        {
            while (time > 0f)
            {
                yield return null;
                time -= scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            }

            run();
        }

        internal static IEnumerator RunByConditionRoutine(Func<bool> condition, Action run)
        {
            while (!condition()) { yield return null; }

            run();
        }

        internal static IEnumerator RunAfterFramesRoutine(int frames, Action run)
        {
            while (frames > 0)
            {
                frames--;
                yield return null;
            }

            run();
        }

        internal static IEnumerator RunWhileRoutine(Func<bool> condition, Action run)
        {
            while (condition())
            {
                run();
                yield return null;
            }
        }
    }
}
