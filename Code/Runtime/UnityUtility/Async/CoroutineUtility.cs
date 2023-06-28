using System;
using System.Collections;
using UnityEngine;

namespace UnityUtility.Async
{
    internal static class CoroutineUtility
    {
        public static IEnumerator GetRunDelayedRoutine(float seconds, Action run, bool scaledTime)
        {
            while (seconds > 0f)
            {
                yield return null;
                seconds -= scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            }

            run();
        }

        public static IEnumerator GetRunByConditionRoutine(Func<bool> condition, Action run)
        {
            while (!condition())
            {
                yield return null;
            }

            run();
        }

        public static IEnumerator GetRunAfterFramesRoutine(int frames, Action run)
        {
            while (frames > 0)
            {
                frames--;
                yield return null;
            }

            run();
        }

        public static IEnumerator GetRunWhileRoutine(Func<bool> condition, Action run)
        {
            while (condition())
            {
                run();
                yield return null;
            }
        }
    }
}
