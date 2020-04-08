using System;
using System.Collections;
using System.Collections.Generic;
using UnityUtility.Collections;

namespace UnityUtility.Async
{
    internal class RoutineRunner : Script, Poolable
    {
        private const string EXCEPTION_TEXT = "Task cannot be stopped. Check " + TaskSystem.SYSTEM_NAME + " settings.";

        private long m_id;

        private RoutineIterator m_iterator;
        private Queue<IEnumerator> m_queue;
        private TaskFactory m_owner;

        private Action m_update;

        public bool IsPaused
        {
            get { return m_iterator.IsPaused; }
        }

        public long Id
        {
            get { return m_id; }
        }

        private void Awake()
        {
            m_iterator = new RoutineIterator(this);
            m_queue = new Queue<IEnumerator>();

            m_update = () =>
            {
                if (m_iterator.IsEmpty && m_queue.Count == 0)
                    m_owner.Release(this);
            };
        }

        public void SetUp(TaskFactory owner)
        {
            m_owner = owner;
            f_init();

            if (m_owner.CanBeStoppedGlobally)
                m_owner.StopTasks_Event += Stop;
        }

        private void OnDestroy()
        {
            ApplicationUtility.OnUpdate_Event -= m_update;
            if (m_owner.CanBeStoppedGlobally)
                m_owner.StopTasks_Event -= Stop;
        }

        // - - //

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

            if (m_iterator.IsEmpty && m_queue.Count > 0)
            {
                f_runAsync(m_queue.Dequeue());
            }
        }

        public void SkipCurrent()
        {
            if (!m_owner.CanBeStopped)
            {
                throw new InvalidOperationException(EXCEPTION_TEXT);
            }

            StopAllCoroutines();
            f_onCoroutineEnded();
        }

        public void Stop()
        {
            if (!m_owner.CanBeStopped)
            {
                throw new InvalidOperationException(EXCEPTION_TEXT);
            }

            if (m_id == 0L)
            {
                return;
            }

            StopAllCoroutines();
            m_queue.Clear();
            f_onCoroutineEnded();
        }

        // - - //

        public void OnCoroutineEnded()
        {
            f_onCoroutineEnded();
        }

        // - - //

        private void f_onCoroutineEnded()
        {
            m_iterator.Reset();

            if (m_queue.Count > 0)
                f_runAsync(m_queue.Dequeue());
            else
                m_id = 0L;
        }

        private void f_init()
        {
            m_id = m_owner.GetNewId();
            ApplicationUtility.OnUpdate_Event += m_update;
        }

        private void f_runAsync(IEnumerator routine)
        {
            m_iterator.Fill(routine);
            StartCoroutine(m_iterator);
        }

        #region IPoolable
        void Poolable.Reinit()
        {
            f_init();
        }

        void Poolable.CleanUp()
        {
            ApplicationUtility.OnUpdate_Event -= m_update;
        }
        #endregion
    }
}
