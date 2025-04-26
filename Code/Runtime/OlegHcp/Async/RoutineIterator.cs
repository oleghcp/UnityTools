using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using OlegHcp.Pool;

namespace OlegHcp.Async
{
    internal class RoutineIterator : IEnumerator, IPoolable
    {
        private readonly TaskRunner _owner;

        private long _id;
        private bool _unstoppable;
        private int _index;

        private IEnumerator _curRoutine;
        private bool _isStopped;
        private CancellationToken _token;

        public long Id => _id;
        public bool CanBeStopped => !_unstoppable;
        public int Index => _index;
        public bool IsEmpty => _curRoutine == null;
        public object Current => _curRoutine.Current;

#if UNITY_EDITOR
        public string StackTrace { get; private set; }
#endif

        public RoutineIterator(TaskRunner owner)
        {
            _owner = owner;
        }

        public void Initialize(IEnumerator routine, long id, bool unstoppable, in CancellationToken token)
        {
#if UNITY_EDITOR
            StackTrace = Environment.StackTrace;
#endif
            _id = id;
            _unstoppable = unstoppable;
            _curRoutine = routine;
            _token = token;
        }

        public void SetIndex(int index)
        {
            _index = index;
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
                _owner.OnCoroutineInterrupted();
                return false;
            }

            if (_curRoutine.MoveNext())
            {
                return true;
            }

            OnCoroutineEndedInternal();
            _owner.OnCoroutineEnded();
            return false;
        }

        public void Reset()
        {
            CleanUp();
        }

        void IPoolable.Reinit() { }

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
            _index = 0;
            _unstoppable = false;
            _curRoutine = null;
            _token = default;
            _isStopped = false;
        }
    }
}
