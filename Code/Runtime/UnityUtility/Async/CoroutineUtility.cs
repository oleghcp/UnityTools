using System;
using System.Collections;
using UnityEngine;

namespace UnityUtility.Async
{
    internal static class CoroutineUtility
    {
        public static IEnumerator RunDelayedRoutine(float time, Action run, bool scaledTime)
        {
            while (time > 0f)
            {
                yield return null;
                time -= scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            }

            run();
        }

        public static IEnumerator RunByConditionRoutine(Func<bool> condition, Action run)
        {
            while (!condition())
            {
                yield return null;
            }

            run();
        }

        public static IEnumerator RunAfterFramesRoutine(int frames, Action run)
        {
            while (frames > 0)
            {
                frames--;
                yield return null;
            }

            run();
        }

        public static IEnumerator RunWhileRoutine(Func<bool> condition, Action run)
        {
            while (condition())
            {
                run();
                yield return null;
            }
        }
    }
}
