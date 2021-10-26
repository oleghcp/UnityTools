using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtilityTools;

namespace UnityUtility.Async
{
    internal class RoutineRunner : MonoBehaviour, IPoolable
    {
        private RoutineIterator _iterator;
        private Queue<IEnumerator> _queue;
        private TaskFactory _owner;

        private long _id;
        private bool _isRunning;

        public bool IsPaused => _iterator.IsPaused;
        public long Id => _id;

        private void Awake()
        {
            _iterator = new RoutineIterator(this);
            _queue = new Queue<IEnumerator>();
        }

        public void SetUp(TaskFactory owner)
        {
            _owner = owner;
            _id = _owner.IdProvider.GetNewId();

            if (_owner.CanBeStoppedGlobally)
                _owner.StopTasks_Event += OnGloballyStoped;
        }

        private void Update()
        {
            if (!_isRunning)
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

        public void Add(IEnumerator routine)
        {
            _queue.Enqueue(routine);

            if (_iterator.IsEmpty)
                RunAsyncInternal(_queue.Dequeue());
        }

        public void SkipCurrent()
        {
            _iterator.Stop();
        }

        public void Stop()
        {
            _queue.Clear();
            _iterator.Stop();
        }

        // - - //

        public void OnCoroutineInterrupted(bool byToken)
        {
            if (!_owner.CanBeStopped)
                throw Errors.CannotStopTask();

            if (byToken)
                _queue.Clear();

            OnCoroutineEnded();
        }

        public void OnCoroutineEnded()
        {
            if (_queue.Count > 0)
                RunAsyncInternal(_queue.Dequeue());
            else
                _isRunning = false;
        }

        // - - //

        private void OnGloballyStoped()
        {
            if (_id != 0L)
                Stop();
        }

        private void RunAsyncInternal(IEnumerator routine)
        {
            _isRunning = true;
            _iterator.Fill(routine);
            StartCoroutine(_iterator);
        }

        #region IPoolable
        void IPoolable.Reinit()
        {
            _id = _owner.IdProvider.GetNewId();
            enabled = true;
        }

        void IPoolable.CleanUp()
        {
            _id = 0L;
            _iterator.Reset();
            enabled = false;
        }
        #endregion
    }
}
