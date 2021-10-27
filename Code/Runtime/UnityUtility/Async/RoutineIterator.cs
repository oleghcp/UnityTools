using System.Collections;
using System.Threading;

namespace UnityUtility.Async
{
    internal class RoutineIterator : IEnumerator
    {
        private readonly RoutineRunner _owner;

        private IEnumerator _curRoutine;
        private bool _isPaused;
        private bool _isStopped;
        private CancellationToken _token;

        public bool IsEmpty => _curRoutine == null;
        public bool IsPaused => _isPaused;

        public RoutineIterator(RoutineRunner owner)
        {
            _owner = owner;
        }

        public void Initialize(IEnumerator routine, in CancellationToken token, bool paused)
        {
            _curRoutine = routine;
            _token = token;
            _isPaused = paused;
        }

        public void Pause(bool value)
        {
            _isPaused = value;
        }

        public void Stop()
        {
            _isStopped = true;
        }

        public void Reset()
        {
            _curRoutine = null;
            _isPaused = false;
            _isStopped = false;
            _token = default;
        }

        object IEnumerator.Current => _curRoutine.Current;

        bool IEnumerator.MoveNext()
        {
            if (_isStopped || _token.IsCancellationRequested)
            {
                _owner.OnCoroutineInterrupted();
                return false;
            }

            if (_isPaused || _curRoutine.MoveNext())
            {
                return true;
            }

            _owner.OnCoroutineEnded();
            return false;
        }
    }
}
