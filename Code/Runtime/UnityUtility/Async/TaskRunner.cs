using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityUtility.CSharp.Collections;
using UnityUtility.Pool;

namespace UnityUtility.Async
{
    internal class TaskRunner : MonoBehaviour, IPoolable
    {
        public event Action<object> OnCompleted_Event;
        public event Action OnInterrupted_Event;

        private RoutineIterator _iterator;
        private TaskDispatcher _owner;
        private List<TaskInfo> _continues;

        private bool _enabled = true;
        private long _id;

        internal TaskDispatcher Owner => _owner;
        public bool IsWaiting => _iterator.IsWaiting;
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

        public TaskInfo RunAsync(IEnumerator routine, in CancellationToken token, bool sleeping = false)
        {
            _id = TaskSystem.IdProvider.GetNewId();
            _iterator.Initialize(routine, token, sleeping);
            TaskInfo task = new TaskInfo(this);
            StartCoroutine(_iterator);
            return task;
        }

        public TaskInfo ContinueWith(IEnumerator routine, in CancellationToken token)
        {
            if (_continues == null)
                _continues = new List<TaskInfo>();
            return _continues.Place(_owner.GetRunner().RunAsync(routine, token, true));
        }

        public void WakeUp()
        {
            _iterator.WakeUp();
        }

        public void Stop()
        {
            _iterator.Stop();
        }

        public void OnCoroutineInterrupted()
        {
            OnCoroutineEndedInternal();

            try { OnInterrupted_Event?.Invoke(); }
            finally { OnInterrupted_Event = null; }
        }

        public void OnCoroutineEnded()
        {
            OnCoroutineEndedInternal();

            try { OnCompleted_Event?.Invoke(_iterator.Current); }
            finally { OnCompleted_Event = null; }
        }

        private void OnCoroutineEndedInternal()
        {
            _id = 0L;

            if (_continues.HasAnyData())
            {
                foreach (TaskInfo item in _continues)
                {
                    item.WakeUp();
                }
                _continues.Clear();
            }
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
