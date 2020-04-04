using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.Scripts;

namespace UnityUtility.Async
{
    [DisallowMultipleComponent]
    internal class RoutineRunner : Script, ITask, Poolable
    {
        private long m_id;

        private RoutineIterator m_iterator;
        private Queue<IEnumerator> m_queue;
        private Action m_update;
        private ITaskFactory _owner;

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
                    _owner.Release(this);
            };
        }

        public void SetUp(ITaskFactory owner)
        {
            _owner = owner;
            f_init();
        }

        private void OnDestroy()
        {
            Updater.Frame_Event -= m_update;
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
            StopAllCoroutines();
            f_onCoroutineEnded();
        }

        public void Stop()
        {
            StopAllCoroutines();
            m_queue.Clear();
            f_onCoroutineEnded();
        }

        // - - //

        void ITask.OnCoroutineEnded()
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
                m_id = 0;
        }

        private void f_init()
        {
            m_id = _owner.GetNewId();
            Updater.Frame_Event += m_update;
        }

        private void f_runAsync(IEnumerator routine)
        {
            m_iterator.Fill(routine);
            StartCoroutine(m_iterator);
        }

        #region IPoolable
        void Poolable.Reinit()
        {
            gameObject.SetActive(true);
            f_init();
        }

        void Poolable.CleanUp()
        {
            gameObject.SetActive(false);
            Updater.Frame_Event -= m_update;
        }
        #endregion
    }
}
