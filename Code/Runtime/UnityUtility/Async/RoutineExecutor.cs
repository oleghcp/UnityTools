using System;
using System.Collections;
using System.Collections.Generic;
using UU.Collections;
using UU.Scripts;

namespace UU.Async
{
    internal class RoutineExecutor : Script, Poolable
    {
        internal uint ID;

        private RoutineEnumerator m_routine;
        private Queue<IEnumerator> m_queue;
        private Action m_update;

        internal bool IsPaused
        {
            get { return m_routine.IsPaused; }
        }

        private void Awake()
        {
            m_routine = new RoutineEnumerator(this);
            m_queue = new Queue<IEnumerator>();

            m_update = () =>
            {
                if (m_routine.IsEmpty && m_queue.Count == 0)
                    AsyncStuffPool.Return(this);
            };

            f_init();
        }

        private void OnDestroy()
        {
            Updater.Frame_Event -= m_update;
        }

        // - - //

        internal TaskInfo RunAsync(IEnumerator routine)
        {
            f_runAsync(routine);
            return new TaskInfo(this);
        }

        internal void Pause()
        {
            m_routine.Pause(true);
        }

        internal void Resume()
        {
            m_routine.Pause(false);
        }

        internal void Add(IEnumerator routine)
        {
            m_queue.Enqueue(routine);
        }

        internal void StartRunning()
        {
            if (m_routine.IsEmpty && m_queue.Count > 0)
            {
                f_runAsync(m_queue.Dequeue());
            }
        }

        internal void SkipCurrent()
        {
            StopAllCoroutines();
            OnCoroutineEnded();
        }

        internal void Stop()
        {
            StopAllCoroutines();
            m_queue.Clear();
            OnCoroutineEnded();
        }

        // - - //

        internal void OnCoroutineEnded()
        {
            m_routine.Reset();

            if (m_queue.Count > 0)
                f_runAsync(m_queue.Dequeue());
            else
                ID = 0;
        }

        // - - //

        private void f_init()
        {
            ID = AsyncStuffPool.GetNewId();
            Updater.Frame_Event += m_update;
        }

        private void f_runAsync(IEnumerator routine)
        {
            m_routine.Fill(routine);
            StartCoroutine(m_routine);
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
