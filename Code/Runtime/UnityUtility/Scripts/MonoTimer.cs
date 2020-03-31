using System;
using System.Collections;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtility.Scripts
{
    public sealed class MonoTimer : Script, Timer
    {
        #region IEnumerator impelementation
        private class Enumerator : IEnumerator
        {
            public Action Callback;
            public bool IsRunning;
            public bool GlobalScale = true;
            public float WaitTime = 1f;
            public float CurTime = 1f;
            public float TimeScale = 1f;

            //IEnumerator//

            public object Current
            {
                get { return null; }
            }

            public bool MoveNext()
            {
                if (IsRunning)
                {
                    if (CurTime >= WaitTime)
                    {
                        IsRunning = false;
                        Callback?.Invoke();
                    }
                    else
                    {
                        CurTime += (GlobalScale ? Time.deltaTime : Time.unscaledDeltaTime) * TimeScale;
                    }
                }

                return IsRunning;
            }

            public void Reset()
            {
                CurTime = 0f;
            }
        }
        #endregion

        private Enumerator mEnumerator = new Enumerator();

        public bool IsRunning
        {
            get { return mEnumerator.IsRunning; }
        }

        public float TargetTime
        {
            get { return mEnumerator.WaitTime; }
        }

        public float CurTime
        {
            get { return mEnumerator.CurTime.CutAfter(mEnumerator.WaitTime); }
        }

        public float Progress
        {
            get { return CurTime / TargetTime; }
        }

        public float TimeScale
        {
            get { return mEnumerator.TimeScale; }
            set { mEnumerator.TimeScale = value; }
        }

        public bool ConsiderGlobalTimeScale
        {
            get { return mEnumerator.GlobalScale; }
            set { mEnumerator.GlobalScale = value; }
        }

        public void Prolong(float addTime)
        {
            mEnumerator.WaitTime += addTime;
        }

        public void InitCallback(Action callback)
        {
            mEnumerator.Callback = callback;
        }

        public void StartCountdown(float time, Action callback)
        {
            InitCallback(callback);

            fStart(time);
        }

        public void StartCountdown(float time, float timeScale = 1f)
        {
            mEnumerator.TimeScale = 1f;
            fStart(time);
        }

        public void StopCountdown()
        {
            StopAllCoroutines();
            mEnumerator.IsRunning = false;
        }

        // -- //

        private void fStart(float time)
        {
            mEnumerator.Reset();
            mEnumerator.WaitTime = time;

            if (time > 0f)
            {
                mEnumerator.IsRunning = true;
                StartCoroutine(mEnumerator);
            }
            else
            {
                mEnumerator.Callback();
            }
        }
    }
}
