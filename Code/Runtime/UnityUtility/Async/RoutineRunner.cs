using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtilityTools;

namespace UnityUtility.Async
{
    internal class RoutineRunner : MonoBehaviour, IPoolable
    {
        private RoutineIterator m_iterator;
        private Queue<IEnumerator> m_queue;
        private TaskFactory m_owner;

        private long m_id;

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
        }

        public void SetUp(TaskFactory owner)
        {
            m_owner = owner;
            f_init();

            if (m_owner.CanBeStoppedGlobally)
                m_owner.StopTasks_Event += OnGloballyStoped;
        }

        private void OnDestroy()
        {
            if (m_owner.CanBeStoppedGlobally)
                m_owner.StopTasks_Event -= OnGloballyStoped;
        }

        // - - //

        public TaskInfo RunAsync(IEnumerator routine, in CancellationToken token)
        {
            f_runAsync(routine, token);
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
                throw Errors.CannotStopTask();

            StopAllCoroutines();
            f_onCoroutineEnded();
        }

        public void Stop()
        {
            if (!m_owner.CanBeStopped)
                throw Errors.CannotStopTask();

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

        private void OnGloballyStoped()
        {
            if (m_id == 0L)
                return;

            Stop();
        }

        private void f_onCoroutineEnded()
        {
            if (m_queue.Count > 0)
            {
                f_runAsync(m_queue.Dequeue());
            }
            else
            {
                m_owner.Release(this);
            }
        }

        private void f_init()
        {
            m_id = m_owner.IdProvider.GetNewId();
        }

        private void f_runAsync(IEnumerator routine)
        {
            m_iterator.Fill(routine);
            StartCoroutine(m_iterator);
        }

        private void f_runAsync(IEnumerator routine, CancellationToken token)
        {
            m_iterator.AddToken(token);
            m_iterator.Fill(routine);
            StartCoroutine(m_iterator);
        }

        #region IPoolable
        void IPoolable.Reinit()
        {
            f_init();
        }

        void IPoolable.CleanUp()
        {
            m_iterator.Reset();
            m_id = 0L;
        }
        #endregion
    }
}
