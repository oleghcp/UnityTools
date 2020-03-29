using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace UU.Async
{
    public struct TaskInfo : IEquatable<TaskInfo>
    {
        private readonly ulong m_id;
        private readonly ITask m_task;

        /// <summary>
        /// Provides task ID.
        /// </summary>
        public ulong TaskId
        {
            get { return m_id; }
        }

        /// <summary>
        /// Returns true if the task is alive. The task is alive while it runs.
        /// </summary>
        public bool IsAlive
        {
            get { return f_isAlive(); }
        }

        public bool IsPaused
        {
            get { return f_isAlive() && m_task.IsPaused; }
        }

        internal TaskInfo(ITask runner)
        {
            m_task = runner;
            m_id = runner.Id;
        }

        /// <summary>
        /// Pauses the task.
        /// </summary>
        public void Pause()
        {
            if (f_isAlive()) { m_task.Pause(); }
        }

        /// <summary>
        /// Resumes paused task.
        /// </summary>
        public void Resume()
        {
            if (f_isAlive()) { m_task.Resume(); }
        }

        /// <summary>
        /// Skips the current coroutine at the queue.
        /// </summary>
        public void SkipCurrent()
        {
            if (f_isAlive()) { m_task.SkipCurrent(); }
        }

        /// <summary>
        /// Stops the task and marks it as non-alive.
        /// </summary>
        public void Stop()
        {
            if (f_isAlive()) { m_task.Stop(); }
        }

        /// <summary>
        /// Adds new task to the queue. It is allowed if the current task is alive.
        /// </summary>
        public void ContinueWith(IEnumerator routine)
        {
            if (f_isAlive())
            {
                m_task.Add(routine);
                m_task.StartRunning();
            }
            else
            {
                throw new InvalidOperationException("Task is not alive.");
            }
        }

        // - - //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool f_isAlive()
        {
            return UnityObjectUtility.IsAlive(m_task) && m_task.Id == m_id;
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is TaskInfo && this == (TaskInfo)obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(TaskInfo other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return m_id.GetHashCode();
        }

        public static bool operator ==(TaskInfo a, TaskInfo b)
        {
            return a.m_id == b.m_id;
        }

        public static bool operator !=(TaskInfo a, TaskInfo b)
        {
            return a.m_id != b.m_id;
        }
    }
}
