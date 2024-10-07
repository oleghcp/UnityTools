using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using OlegHcp.Pool;
using UnityEngine;

namespace OlegHcp.Async
{
    internal class TaskRunner : MonoBehaviour, IPoolable
    {
        public event Action<TaskResult> OnCompleted1_Event;
        public event Action OnCompleted2_Event;

        private RoutineIterator _iterator;
        private TaskDispatcher _owner;
        private long _id;
        private bool _enabled = true;
        private bool _unstoppable;

#if UNITY_EDITOR
        public string StackTrace { get; private set; }
#endif
        public TaskDispatcher Owner => _owner;
        public long Id => _id;
        public bool CanBeStopped => !_unstoppable;

        public TaskRunner SetUp(TaskDispatcher owner)
        {
            _iterator = new RoutineIterator(this);
            _owner = owner;
            return this;
        }

        public void Refresh()
        {
            if (_enabled && _id == 0L)
                _owner.ReleaseRunner(this);
        }

        public TaskInfo RunAsync(IEnumerator routine, long id, bool unstoppable, in CancellationToken token)
        {
#if UNITY_EDITOR
            StackTrace = Environment.StackTrace;
#endif
            _id = id;
            _unstoppable = unstoppable;
            _iterator.Initialize(routine, token);
            TaskInfo task = new TaskInfo(this);
            StartCoroutine(_iterator);
            return task;
        }

        public void Stop()
        {
            if (_unstoppable)
                throw new InvalidOperationException("Task cannot be stopped.");

            _iterator.Stop();
        }

        public void OnCoroutineInterrupted()
        {
            OnCoroutineEndedInternal();
            OnCompleted2_Event = null;

            if (OnCompleted1_Event == null)
                return;

            try { OnCompleted1_Event(default); }
            finally { OnCompleted1_Event = null; }
        }

        public void OnCoroutineEnded()
        {
            OnCoroutineEndedInternal();

            if (OnCompleted1_Event != null)
            {
                try { OnCompleted1_Event(new TaskResult(_iterator.Current, true)); }
                finally { OnCompleted1_Event = null; }
            }

            if (OnCompleted2_Event != null)
            {
                try { OnCompleted2_Event(); }
                finally { OnCompleted2_Event = null; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnCoroutineEndedInternal()
        {
            _id = 0L;
        }

        void IPoolable.Reinit()
        {
            _enabled = true;
        }

        void IPoolable.CleanUp()
        {
            _iterator.Reset();
            _enabled = false;
#if UNITY_EDITOR
            StackTrace = null;
#endif
        }
    }
}
