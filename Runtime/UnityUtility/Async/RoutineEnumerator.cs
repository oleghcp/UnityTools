using System.Collections;

namespace UU.Async
{
    internal class RoutineEnumerator : IEnumerator
    {
        private readonly RoutineExecutor OWNER;

        private IEnumerator m_cur;
        private bool m_isPaused;

        internal bool IsEmpty
        {
            get { return m_cur == null; }
        }

        internal bool IsPaused
        {
            get { return m_isPaused; }
        }

        internal RoutineEnumerator(RoutineExecutor owner)
        {
            OWNER = owner;
        }

        internal void Fill(IEnumerator routine)
        {
            m_cur = routine;
        }

        internal void Pause(bool value)
        {
            m_isPaused = value;
        }

        public void Reset()
        {
            m_cur = null;
            m_isPaused = false;
        }

        object IEnumerator.Current
        {
            get { return m_cur.Current; }
        }

        bool IEnumerator.MoveNext()
        {
            if (m_isPaused) { return true; }

            bool canMove = m_cur.MoveNext();

            if (!canMove) { OWNER.OnCoroutineEnded(); }

            return canMove;
        }
    }
}
