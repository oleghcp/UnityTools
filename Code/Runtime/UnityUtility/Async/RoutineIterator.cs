using System.Collections;
using System.Threading;

namespace UnityUtility.Async
{
    internal class RoutineIterator : IEnumerator
    {
        private readonly TaskRunner _owner;

        private IEnumerator _curRoutine;
        private bool _isWaitingForPrev;
        private bool _isStopped;
        private CancellationToken _token;

        public bool IsEmpty => _curRoutine == null;
        public bool IsWaiting => _isWaitingForPrev;
        public object Current => _curRoutine.Current;

        public RoutineIterator(TaskRunner owner)
        {
            _owner = owner;
        }

        public void Initialize(IEnumerator routine, in CancellationToken token, bool waiting)
        {
            _curRoutine = routine;
            _token = token;
            _isWaitingForPrev = waiting;
        }

        public void WakeUp()
        {
            _isWaitingForPrev = false;
        }

        public void Stop()
        {
            _isStopped = true;
        }

        public void Reset()
        {
            _curRoutine = null;
            _isWaitingForPrev = false;
            _isStopped = false;
            _token = default;
        }

        bool IEnumerator.MoveNext()
        {
            if (_isStopped || _token.IsCancellationRequested)
            {
                _owner.OnCoroutineInterrupted();
                return false;
            }

            if (_isWaitingForPrev || _curRoutine.MoveNext())
            {
                return true;
            }

            _owner.OnCoroutineEnded();
            return false;
        }
    }
}
