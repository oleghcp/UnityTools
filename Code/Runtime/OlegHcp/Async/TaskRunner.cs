using System;
using System.Collections;
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

        public TaskDispatcher Owner => _owner;
        public long Id => _iterator.Id;
        public bool CanBeStopped => !_iterator.CanBeStopped;

#if UNITY_EDITOR
        public string StackTrace => _iterator.StackTrace;
#endif

        public TaskRunner SetUp(TaskDispatcher owner)
        {
            _iterator = new RoutineIterator(this);
            _owner = owner;
            return this;
        }

        public void SetIndex(int index)
        {
            _iterator.SetIndex(index);
        }

        public void Refresh()
        {
            if (_iterator.Id == 0L)
                _owner.ReleaseRunner(this, _iterator.Index);
        }

        public TaskInfo RunAsync(IEnumerator routine, long id, bool unstoppable, in CancellationToken token)
        {
            _iterator.Initialize(routine, id, unstoppable, token);
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
            OnCompleted2_Event = null;

            if (OnCompleted1_Event == null)
                return;

            try { OnCompleted1_Event(default); }
            finally { OnCompleted1_Event = null; }
        }

        public void OnCoroutineEnded()
        {
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

        void IPoolable.Reinit() { }

        void IPoolable.CleanUp()
        {
            _iterator.Reset();
        }
    }
}
