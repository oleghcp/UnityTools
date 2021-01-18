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
            m_id = m_owner.IdProvider.GetNewId();

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
            m_iterator.AddToken(token);
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

            if (m_iterator.IsEmpty)
                f_runAsync(m_queue.Dequeue());
        }

        public void SkipCurrent()
        {
            m_iterator.Stop();
        }

        public void Stop()
        {
            m_queue.Clear();
            m_iterator.Stop();
        }

        // - - //

        public void OnCoroutineEnded()
        {
            if (m_queue.Count > 0)
                f_runAsync(m_queue.Dequeue());
            else
                m_owner.Release(this);
        }

        public void OnCoroutineInterrupted(bool byToken)
        {
            if (!m_owner.CanBeStopped)
                throw Errors.CannotStopTask();

            if (byToken)
                m_queue.Clear();

            OnCoroutineEnded();
        }

        // - - //

        private void OnGloballyStoped()
        {
            if (m_id != 0L)
                Stop();
        }

        private void f_runAsync(IEnumerator routine)
        {
            m_iterator.Fill(routine);
            StartCoroutine(m_iterator);
        }

        #region IPoolable
        void IPoolable.Reinit()
        {
            m_id = m_owner.IdProvider.GetNewId();
        }

        void IPoolable.CleanUp()
        {
            m_id = 0L;
            m_iterator.Reset();
        }
        #endregion
    }
}
