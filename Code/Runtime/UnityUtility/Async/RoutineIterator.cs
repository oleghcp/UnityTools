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

        internal bool IsEmpty => _curRoutine == null;
        internal bool IsPaused => _isPaused;

        internal RoutineIterator(RoutineRunner owner)
        {
            _owner = owner;
        }

        internal void Fill(IEnumerator routine)
        {
            _curRoutine = routine;
        }

        internal void AddToken(in CancellationToken token)
        {
            _token = token;
        }

        internal void Pause(bool value)
        {
            _isPaused = value;
        }

        internal void Stop()
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
            if (_isStopped)
            {
                _owner.OnCoroutineInterrupted(false);
                return false;
            }

            if (_token.IsCancellationRequested)
            {
                _owner.OnCoroutineInterrupted(true);
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
