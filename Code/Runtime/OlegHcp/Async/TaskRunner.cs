using System;
using System.Collections;
using System.Threading;
using OlegHcp.Pool;
using UnityEngine;

namespace OlegHcp.Async
{
    internal class TaskRunner : MonoBehaviour, IPoolable
    {
        public event Action<TaskResult> OnCompleted1_Event
        {
            add { _iterator.OnCompleted1_Event += value; }
            remove { _iterator.OnCompleted1_Event -= value; }
        }

        public event Action OnCompleted2_Event
        {
            add { _iterator.OnCompleted2_Event += value; }
            remove { _iterator.OnCompleted2_Event -= value; }
        }

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
            _iterator = new RoutineIterator();
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

        void IPoolable.Reinit() { }

        void IPoolable.CleanUp()
        {
            _iterator.Reset();
        }
    }
}
