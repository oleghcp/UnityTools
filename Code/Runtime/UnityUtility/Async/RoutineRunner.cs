using System.Collections;
using System.Threading;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtilityTools;

namespace UnityUtility.Async
{
    internal class RoutineRunner : MonoBehaviour, IPoolable
    {
        private RoutineIterator _iterator;
        private TaskFactory _owner;

        private long _id;

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

        public TaskInfo RunAsync(IEnumerator routine, in CancellationToken token)
        {
            _id = _owner.IdProvider.GetNewId();
            _iterator.AddToken(token);
            RunAsyncInternal(routine);
            return new TaskInfo(this);
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
        }

        // - - //

        private void OnGloballyStoped()
        {
            if (_id != 0L)
                Stop();
        }

        private void RunAsyncInternal(IEnumerator routine)
        {
            _iterator.Fill(routine);
            StartCoroutine(_iterator);
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
