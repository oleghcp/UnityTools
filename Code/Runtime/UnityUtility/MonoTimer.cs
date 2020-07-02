using System;
using System.Collections;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtility
{
    public sealed class MonoTimer : MonoBehaviour, ITimer
    {
        #region IEnumerator impelementation
        private class Enumerator : IEnumerator
        {
            public Action Callback;
            public bool IsRunning;
            public bool GlobalScale = true;
            public float WaitTime = 1f;
            public float CurrentTime = 1f;
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
                    if (CurrentTime >= WaitTime)
                    {
                        IsRunning = false;
                        Callback?.Invoke();
                    }
                    else
                    {
                        CurrentTime += (GlobalScale ? Time.deltaTime : Time.unscaledDeltaTime) * TimeScale;
                    }
                }

                return IsRunning;
            }

            public void Reset()
            {
                CurrentTime = 0f;
            }
        }
        #endregion

        private Enumerator m_routine = new Enumerator();

        public bool IsRunning
        {
            get { return m_routine.IsRunning; }
        }

        public float TargetTime
        {
            get { return m_routine.WaitTime; }
        }

        public float CurrentTime
        {
            get { return m_routine.CurrentTime.CutAfter(m_routine.WaitTime); }
        }

        public float Progress
        {
            get { return CurrentTime / TargetTime; }
        }

        public float TimeScale
        {
            get { return m_routine.TimeScale; }
            set { m_routine.TimeScale = value; }
        }

        public bool ConsiderGlobalTimeScale
        {
            get { return m_routine.GlobalScale; }
            set { m_routine.GlobalScale = value; }
        }

        public void Prolong(float addTime)
        {
            m_routine.WaitTime += addTime;
        }

        public void InitCallback(Action callback)
        {
            m_routine.Callback = callback;
        }

        public void StartCountdown(float time, Action callback)
        {
            InitCallback(callback);

            f_start(time);
        }

        public void StartCountdown(float time, float timeScale = 1f)
        {
            m_routine.TimeScale = timeScale;
            f_start(time);
        }

        public void StopCountdown()
        {
            StopAllCoroutines();
            m_routine.IsRunning = false;
        }

        // -- //

        private void f_start(float time)
        {
            m_routine.Reset();
            m_routine.WaitTime = time;

            if (time > 0f)
            {
                m_routine.IsRunning = true;
                StartCoroutine(m_routine);
            }
            else
            {
                m_routine.Callback();
            }
        }
    }
}
