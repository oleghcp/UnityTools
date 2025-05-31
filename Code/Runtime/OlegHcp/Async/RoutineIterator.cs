using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using OlegHcp.Pool;

namespace OlegHcp.Async
{
    internal class RoutineIterator : IEnumerator, IPoolable
    {
        public event Action OnCompleted_Event;
        public event Action<TaskResult> OnCompleted2_Event;

        private long _id;
        private bool _unstoppable;
        private int _index;
        private IEnumerator _curRoutine;
        private bool _isStopped;
        private CancellationToken _token;
        private TaskRunner _taskRunner;

        public long Id => _id;
        public bool CanBeStopped => !_unstoppable;
        object IEnumerator.Current => _curRoutine.Current;

#if UNITY_EDITOR
        public string StackTrace { get; private set; }

        public RoutineIterator() { }

        public RoutineIterator(IEnumerator routine, long id, bool unstoppable, in CancellationToken token)
        {
            Initialize(null, routine, id, false, default, token);
        }

        public void InitStackTrace(string stackTrace)
        {
            StackTrace = stackTrace;
        }
#endif

        public void Initialize(TaskRunner taskRunner, IEnumerator routine, long id, bool unstoppable, int index, in CancellationToken token)
        {
            _taskRunner = taskRunner;
            _index = index;

            _curRoutine = routine;
            _id = id;
            _unstoppable = unstoppable;
            _token = token;
        }

        public void Refresh()
        {
            if (_id == 0L)
                _taskRunner.ReleaseRunner(this, _index);
        }

        public void Stop()
        {
            if (_unstoppable)
                throw new InvalidOperationException("Task cannot be stopped.");

            _isStopped = true;
        }

        bool IEnumerator.MoveNext()
        {
            if (_isStopped || _token.IsCancellationRequested)
            {
                OnCoroutineEndedInternal();
                InvokeInterrupted();
                return false;
            }

            if (_curRoutine.MoveNext())
            {
                return true;
            }

            OnCoroutineEndedInternal();
            InvokeEnded();
            return false;
        }

        public void Reset()
        {
            CleanUp();
        }

        void IPoolable.Reinit()
        {

        }

        void IPoolable.CleanUp()
        {
            CleanUp();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnCoroutineEndedInternal()
        {
            _id = 0L;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CleanUp()
        {
            _taskRunner = null;
            _index = 0;

            _curRoutine = null;
            _id = 0L;
            _unstoppable = false;
            _token = default;
            _isStopped = false;
        }

        private void InvokeInterrupted()
        {
            OnCompleted_Event = null;

            if (OnCompleted2_Event != null)
            {
                try { OnCompleted2_Event(default); }
                finally { OnCompleted2_Event = null; }
            }
        }

        private void InvokeEnded()
        {
            if (OnCompleted_Event != null)
            {
                try { OnCompleted_Event(); }
                finally { OnCompleted_Event = null; }
            }

            if (OnCompleted2_Event != null)
            {
                try { OnCompleted2_Event(new TaskResult(_curRoutine.Current, true)); }
                finally { OnCompleted2_Event = null; }
            }
        }
    }
}
