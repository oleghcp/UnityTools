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

            public object Current => null;

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

        private Enumerator _routine = new Enumerator();

        public bool IsRunning => _routine.IsRunning;
        public float TargetTime => _routine.WaitTime;
        public float CurrentTime => _routine.CurrentTime.CutAfter(_routine.WaitTime);
        public float Progress => CurrentTime / TargetTime;

        public float TimeScale
        {
            get => _routine.TimeScale;
            set => _routine.TimeScale = value;
        }

        public bool ConsiderGlobalTimeScale
        {
            get => _routine.GlobalScale;
            set => _routine.GlobalScale = value;
        }

        public void Prolong(float addTime)
        {
            _routine.WaitTime += addTime;
        }

        public void InitCallback(Action callback)
        {
            _routine.Callback = callback;
        }

        public void StartCountdown(float time, Action callback)
        {
            InitCallback(callback);

            StartInternal(time);
        }

        public void StartCountdown(float time, float timeScale = 1f)
        {
            _routine.TimeScale = timeScale;
            StartInternal(time);
        }

        public void StopCountdown()
        {
            StopAllCoroutines();
            _routine.IsRunning = false;
        }

        // -- //

        private void StartInternal(float time)
        {
            _routine.Reset();
            _routine.WaitTime = time;

            if (time > 0f)
            {
                _routine.IsRunning = true;
                StartCoroutine(_routine);
            }
            else
            {
                _routine.Callback();
            }
        }
    }
}
