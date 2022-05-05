using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityUtility.Pool;
using UnityUtilityTools;

namespace UnityUtility.Async
{
    internal class RoutineRunner : MonoBehaviour, IPoolable
    {
        private RoutineIterator _iterator;
        private TaskFactory _owner;
        private List<TaskInfo> _continues = new List<TaskInfo>();

        private long _id;

        internal TaskFactory Owner => _owner;
        public bool IsPaused => _iterator.IsPaused;
        public long Id => _id;

        public RoutineRunner SetUp(TaskFactory owner)
        {
            _iterator = new RoutineIterator(this);
            _owner = owner;
            _id = _owner.IdProvider.GetNewId();

            if (_owner.CanBeStoppedGlobally)
                _owner.StopTasks_Event += OnGloballyStoped;

            return this;
        }

        private void Update()
        {
            if (_id == 0L)
                _owner.Release(this);
        }

        private void OnDestroy()
        {
            if (_owner.CanBeStoppedGlobally)
                _owner.StopTasks_Event -= OnGloballyStoped;
        }

        // - - //

        public TaskInfo RunAsync(IEnumerator routine, in CancellationToken token, bool paused = false)
        {
            _id = _owner.IdProvider.GetNewId();
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

        // - - //

        public void OnCoroutineInterrupted()
        {
            if (!_owner.CanBeStopped)
                throw Errors.CannotStopTask();

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

        // - - //

        private void OnGloballyStoped()
        {
            if (_id != 0L)
                Stop();
        }

        #region IPoolable
        void IPoolable.Reinit()
        {
            enabled = true;
        }

        void IPoolable.CleanUp()
        {
            _iterator.Reset();
            enabled = false;
        }
        #endregion
    }
}
