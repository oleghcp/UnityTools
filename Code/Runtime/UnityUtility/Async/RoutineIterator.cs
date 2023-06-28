using System.Collections;
using System.Threading;

namespace UnityUtility.Async
{
    internal class RoutineIterator : IEnumerator
    {
        private readonly TaskRunner _owner;

        private IEnumerator _curRoutine;
        private bool _isStopped;
        private CancellationToken _token;

        public bool IsEmpty => _curRoutine == null;
        public object Current => _curRoutine.Current;

        public RoutineIterator(TaskRunner owner)
        {
            _owner = owner;
        }

        public void Initialize(IEnumerator routine, in CancellationToken token)
        {
            _curRoutine = routine;
            _token = token;
        }

        public void Stop()
        {
            _isStopped = true;
        }

        public void Reset()
        {
            _curRoutine = null;
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

            if (_curRoutine.MoveNext())
            {
                return true;
            }

            _owner.OnCoroutineEnded();
            return false;
        }
    }
}
