using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OlegHcp.Async
{
    internal class RoutineIterator : IEnumerator
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

        public RoutineIterator(TaskRunner owner)
        {
            _owner = owner;
        }

        public void Initialize(IEnumerator routine, long id, bool unstoppable, in CancellationToken token)
        {
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

        public void Reset()
        {
            _index = 0;
            _unstoppable = false;
            _curRoutine = null;
            _token = default;
            _isStopped = false;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnCoroutineEndedInternal()
        {
            _id = 0L;
        }
    }
}
