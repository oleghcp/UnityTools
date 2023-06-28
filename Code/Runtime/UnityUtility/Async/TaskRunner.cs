using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityUtility.Pool;

namespace UnityUtility.Async
{
    internal class TaskRunner : MonoBehaviour, IPoolable
    {
        public event Action<TaskResult> OnCompleted_Event;

        private RoutineIterator _iterator;
        private TaskDispatcher _owner;
        private bool _enabled = true;
        private long _id;

#if UNITY_EDITOR
        internal string StackTrace { get; private set; }
#endif
        internal TaskDispatcher Owner => _owner;
        public long Id => _id;

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

        public TaskInfo RunAsync(IEnumerator routine, in CancellationToken token)
        {
#if UNITY_EDITOR
            StackTrace = Environment.StackTrace;
#endif

            _id = TaskSystem.IdProvider.GetNewId();
            _iterator.Initialize(routine, token);
            TaskInfo task = new TaskInfo(this);
            StartCoroutine(_iterator);
            return task;
        }

        public void Stop()
        {
            _iterator.Stop();
        }

        public void OnCoroutineInterrupted()
        {
            OnCoroutineEndedInternal();

            if (OnCompleted_Event != null)
            {
                try { OnCompleted_Event(default); }
                finally { OnCompleted_Event = null; }
            }
        }

        public void OnCoroutineEnded()
        {
            OnCoroutineEndedInternal();

            if (OnCompleted_Event != null)
            {
                try { OnCompleted_Event(new TaskResult(_iterator.Current, true)); }
                finally { OnCompleted_Event = null; }
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
        }
    }
}
