using System.Collections;

namespace UnityUtility.Async
{
    internal class RoutineIterator : IEnumerator
    {
        private readonly ITask m_owner;

        private IEnumerator m_curRoutine;
        private bool m_isPaused;

        internal bool IsEmpty
        {
            get { return m_curRoutine == null; }
        }

        internal bool IsPaused
        {
            get { return m_isPaused; }
        }

        internal RoutineIterator(ITask owner)
        {
            m_owner = owner;
        }

        internal void Fill(IEnumerator routine)
        {
            m_curRoutine = routine;
        }

        internal void Pause(bool value)
        {
            m_isPaused = value;
        }

        public void Reset()
        {
            m_curRoutine = null;
            m_isPaused = false;
        }

        object IEnumerator.Current
        {
            get { return m_curRoutine.Current; }
        }

        bool IEnumerator.MoveNext()
        {
            if (m_isPaused) { return true; }

            bool canMove = m_curRoutine.MoveNext();

            if (!canMove) { m_owner.OnCoroutineEnded(); }

            return canMove;
        }
    }
}
