using System.Collections;
using System.Threading;

namespace UnityUtility.Async
{
    internal class RoutineIterator : IEnumerator
    {
        private readonly RoutineRunner m_owner;

        private IEnumerator m_curRoutine;
        private bool m_isPaused;
        private CancellationToken m_token;

        internal bool IsEmpty
        {
            get { return m_curRoutine == null; }
        }

        internal bool IsPaused
        {
            get { return m_isPaused; }
        }

        internal RoutineIterator(RoutineRunner owner)
        {
            m_owner = owner;
        }

        internal void Fill(IEnumerator routine)
        {
            m_curRoutine = routine;
        }

        internal void AddToken(in CancellationToken token)
        {
            m_token = token;
        }

        internal void Pause(bool value)
        {
            m_isPaused = value;
        }

        public void Reset()
        {
            m_curRoutine = null;
            m_isPaused = false;
            m_token = default;
        }

        object IEnumerator.Current
        {
            get { return m_curRoutine.Current; }
        }

        bool IEnumerator.MoveNext()
        {
            if (m_token.IsCancellationRequested)
            {
                m_owner.Stop();
                return false;
            }

            if (m_isPaused || m_curRoutine.MoveNext())
            {
                return true;
            }

            m_owner.OnCoroutineEnded();
            return false;
        }
    }
}
