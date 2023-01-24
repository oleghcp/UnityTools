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
        private RoutineIterator _iterator;
        private TaskDispatcher _owner;
        private List<TaskInfo> _continues = new List<TaskInfo>();

        private bool _enabled = true;
        private long _id;

        internal TaskDispatcher Owner => _owner;
        public bool IsPaused => _iterator.IsPaused;
        public long Id => _id;

        public TaskRunner SetUp(TaskDispatcher owner)
        {
            _iterator = new RoutineIterator(this);
            _owner = owner;
            return this;
        }

        public void Refresh()
        {
            if (_enabled)
            {
                if (_id == 0L)
                    _owner.ReleaseRunner(this);
            }
        }

        // - - //

        public TaskInfo RunAsync(IEnumerator routine, in CancellationToken token, bool paused = false)
        {
            _id = TaskSystem.IdProvider.GetNewId();
            _iterator.Initialize(routine, token, paused);
            StartCoroutine(_iterator);
            return new TaskInfo(this);
        }

        public TaskInfo ContinueWith(IEnumerator routine, in CancellationToken token)
        {
            TaskInfo task = _owner.GetRunner().RunAsync(routine, token, true);
            return _continues.Place(task);
        }

        public void Pause()
        {
            _iterator.Pause(true);
        }

        public void Resume()
        {
            _iterator.Pause(false);
        }

        public void Stop()
        {
            _iterator.Stop();
        }

        public void OnCoroutineInterrupted()
        {
            OnCoroutineEnded();
        }

        public void OnCoroutineEnded()
        {
            _id = 0L;

            if (_continues.Count > 0)
            {
                foreach (TaskInfo item in _continues)
                    item.Resume();
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
