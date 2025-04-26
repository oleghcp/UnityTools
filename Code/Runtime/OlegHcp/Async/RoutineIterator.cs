using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using OlegHcp.Pool;

namespace OlegHcp.Async
{
    internal class RoutineIterator : IEnumerator, IPoolable
    {
        public event Action<TaskResult> OnCompleted1_Event;
        public event Action OnCompleted2_Event;

        private long _id;
        private bool _unstoppable;
        private int _index;

        private IEnumerator _curRoutine;
        private bool _isStopped;
        private CancellationToken _token;
        private TaskDispatcher _taskDispatcher;

        public long Id => _id;
        public bool CanBeStopped => !_unstoppable;
        public bool IsEmpty => _curRoutine == null;
        public object Current => _curRoutine.Current;

#if UNITY_EDITOR
        public string StackTrace { get; private set; }
#endif

        public void Initialize(TaskDispatcher taskDispatcher, IEnumerator routine, long id, bool unstoppable, int index, in CancellationToken token)
        {
#if UNITY_EDITOR
            StackTrace = Environment.StackTrace;
#endif
            _taskDispatcher = taskDispatcher;
            _curRoutine = routine;
            _id = id;
            _unstoppable = unstoppable;
            _index = index;
            _token = token;
        }

        public void Refresh()
        {
            if (_id == 0L)
                _taskDispatcher.ReleaseRunner(this, _index);
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
#if UNITY_EDITOR
            StackTrace = null;
#endif
            _id = 0L;
            _index = 0;
            _unstoppable = false;
            _curRoutine = null;
            _token = default;
            _isStopped = false;
        }

        private void InvokeInterrupted()
        {
            OnCompleted2_Event = null;

            if (OnCompleted1_Event == null)
                return;

            try { OnCompleted1_Event(default); }
            finally { OnCompleted1_Event = null; }
        }

        private void InvokeEnded()
        {
            if (OnCompleted1_Event != null)
            {
                try { OnCompleted1_Event(new TaskResult(_curRoutine.Current, true)); }
                finally { OnCompleted1_Event = null; }
            }

            if (OnCompleted2_Event != null)
            {
                try { OnCompleted2_Event(); }
                finally { OnCompleted2_Event = null; }
            }
        }
    }
}
