using System;
using System.Collections;
using UnityEngine;
using UnityUtility.Async;
using UnityUtility.MathExt;

namespace UnityUtility.Timers
{
    public sealed class MonoTimer : ITimer
    {
        public event Action<ITimer> Elapsed_Event;

        private readonly bool _local;
        private readonly Enumerator _routine;
        private TaskInfo _task;

        public bool IsRunning => _task.IsAlive;
        public float TargetTime => _routine.WaitTime;
        public float CurrentTime => _routine.CurrentTime.ClampMax(_routine.WaitTime);
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
            _routine = new Enumerator(this);
        }

        public void Prolong(float extraTime)
        {
            _routine.WaitTime += extraTime;
        }

        public void Start(float time, float timeScale)
        {
            _routine.TimeScale = timeScale;
            StartInternal(time);
        }

        public void Start(float time)
        {
            _routine.TimeScale = 1f;
            StartInternal(time);
        }

        public void Stop()
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
                Elapsed_Event?.Invoke(this);
            }
        }

        #region IEnumerator impelementation
        private class Enumerator : IEnumerator
        {
            public bool GlobalScale = true;
            public float WaitTime = 1f;
            public float CurrentTime;
            public float TimeScale = 1f;
            private MonoTimer _owner;

            public object Current => null;

            public Enumerator(MonoTimer owner)
            {
                _owner = owner;
            }

            public bool MoveNext()
            {
                if (CurrentTime >= WaitTime)
                {
                    _owner.Elapsed_Event?.Invoke(_owner);
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
