using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityUtility.Async;
using static Unity.EditorCoroutines.Editor.EditorCoroutineUtility;

namespace UUEditor.Async
{
    internal class RoutineWrapper : ITask
    {
        private long m_id;

        private RoutineIterator m_iterator;
        private Queue<IEnumerator> m_queue;
        private EditorCoroutine m_coroutine;

        public bool IsPaused => m_iterator.IsPaused;

        public long Id => m_id;

        public RoutineWrapper(long id)
        {
            m_id = id;
            m_iterator = new RoutineIterator(this);
            m_queue = new Queue<IEnumerator>();
        }

        public TaskInfo RunAsync(IEnumerator routine)
        {
            f_runAsync(routine);
            return new TaskInfo(this);
        }

        public void Pause()
        {
            m_iterator.Pause(true);
        }

        public void Resume()
        {
            m_iterator.Pause(false);
        }

        public void Add(IEnumerator routine)
        {
            m_queue.Enqueue(routine);
        }

        public void StartRunning()
        {
            if (m_iterator.IsEmpty && m_queue.Count > 0)
            {
                f_runAsync(m_queue.Dequeue());
            }
        }

        public void SkipCurrent()
        {
            StopCoroutine(m_coroutine);
            f_onCoroutineEnded();
        }

        public void Stop()
        {
            StopCoroutine(m_coroutine);
            m_queue.Clear();
            f_onCoroutineEnded();
        }

        void ITask.OnCoroutineEnded()
        {
            f_onCoroutineEnded();
        }

        private void f_onCoroutineEnded()
        {
            m_iterator.Reset();

            if (m_queue.Count > 0)
                f_runAsync(m_queue.Dequeue());
            else
                m_id = 0;
        }

        private void f_runAsync(IEnumerator routine)
        {
            m_iterator.Fill(routine);
            m_coroutine = StartCoroutineOwnerless(m_iterator);
        }
    }
}
