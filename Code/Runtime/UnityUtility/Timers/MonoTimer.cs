using System;
using System.Collections;
using UnityEngine;
using OlegHcp.Async;
using OlegHcp.Mathematics;
using OlegHcp.Tools;

namespace OlegHcp.Timers
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

        public bool ConsiderGlobalTimeScale
        {
            get => _routine.GlobalScale;
            set => _routine.GlobalScale = value;
        }

        public float ExtraTimeScale
        {
            get => _routine.ExtraTimeScale;
            set
            {
                if (value < 0f)
                    throw ThrowErrors.NegativeParameter(nameof(ExtraTimeScale));

                _routine.ExtraTimeScale = value;
            }
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

        public void Start(float time)
        {
            _routine.Reset();
            _routine.WaitTime = time;

            if (time == 0f)
            {
                Elapsed_Event?.Invoke(this);
                return;
            }

            _task = _local ? TaskSystem.StartAsyncLocally(_routine)
                           : TaskSystem.StartAsync(_routine);
        }

        public void Stop()
        {
            _task.Stop();
        }

        #region IEnumerator impelementation
        private class Enumerator : IEnumerator
        {
            public bool GlobalScale = true;
            public float WaitTime = 1f;
            public float CurrentTime;
            public float ExtraTimeScale = 1f;

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

                CurrentTime += (GlobalScale ? Time.deltaTime : Time.unscaledDeltaTime) * ExtraTimeScale;
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
