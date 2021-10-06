using System;
using System.Collections;
using UnityEngine;
using UnityUtility.Async;
using UnityUtility.MathExt;

namespace UnityUtility.Timers
{
    public sealed class MonoTimer : ITimer
    {
        private readonly bool _local;
        private readonly Enumerator _routine = new Enumerator();
        private TaskInfo _task;

        public bool IsRunning => _task.IsAlive;
        public float TargetTime => _routine.WaitTime;
        public float CurrentTime => _routine.CurrentTime.CutAfter(_routine.WaitTime);
        public float Progress => CurrentTime / _routine.WaitTime;

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

        public MonoTimer(bool withinCurrentScene = true)
        {
            _local = withinCurrentScene;
        }

        public void InitCallback(Action callback)
        {
            _routine.Callback = callback;
        }

        public void Prolong(float extraTime)
        {
            _routine.WaitTime += extraTime;
        }

        public void StartCountdown(float time, float timeScale = 1f)
        {
            _routine.TimeScale = timeScale;
            StartInternal(time);
        }

        public void StartCountdown(float time, Action callback)
        {
            InitCallback(callback);
            StartInternal(time);
        }

        public void StopCountdown()
        {
            _task.Stop();
        }

        // -- //

        private void StartInternal(float time)
        {
            _routine.Reset();
            _routine.WaitTime = time;

            if (time > 0f)
            {
                if (_local)
                    _task = TaskSystem.StartAsyncLocally(_routine);
                else
                    _task = TaskSystem.StartAsync(_routine);
            }
            else
            {
                _routine.Callback?.Invoke();
            }
        }

        #region IEnumerator impelementation
        private class Enumerator : IEnumerator
        {
            public Action Callback;
            public bool GlobalScale = true;
            public float WaitTime = 1f;
            public float CurrentTime;
            public float TimeScale = 1f;

            //IEnumerator//

            public object Current => null;

            public bool MoveNext()
            {
                if (CurrentTime >= WaitTime)
                {
                    Callback?.Invoke();
                    return false;
                }

                CurrentTime += (GlobalScale ? Time.deltaTime : Time.unscaledDeltaTime) * TimeScale;
                return true;
            }

            public void Reset()
            {
                CurrentTime = 0f;
            }
        }
        #endregion
    }
}
