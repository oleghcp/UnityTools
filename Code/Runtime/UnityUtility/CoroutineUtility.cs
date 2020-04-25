using System;
using System.Collections;
using UnityEngine;

namespace Assets.Code.Runtime.UnityUtility
{
    internal static class CoroutineUtility
    {
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
