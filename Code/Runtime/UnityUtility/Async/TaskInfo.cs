using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UU.Async
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TaskInfo : IEquatable<TaskInfo>
    {
        private readonly uint m_id;
        private readonly RoutineRunner m_runner;

        /// <summary>
        /// Provides task ID.
        /// </summary>
        public uint TaskId
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
            get { return f_isAlive() && m_runner.IsPaused; }
        }

        internal TaskInfo(RoutineRunner runner)
        {
            m_runner = runner;
            m_id = runner.ID;
        }

        /// <summary>
        /// Pauses the task.
        /// </summary>
        public void Pause()
        {
            if (f_isAlive()) { m_runner.Pause(); }
        }

        /// <summary>
        /// Resumes paused task.
        /// </summary>
        public void Resume()
        {
            if (f_isAlive()) { m_runner.Resume(); }
        }

        /// <summary>
        /// Skips the current coroutine at the queue.
        /// </summary>
        public void SkipCurrent()
        {
            if (f_isAlive()) { m_runner.SkipCurrent(); }
        }

        /// <summary>
        /// Stops the task and marks it as non-alive.
        /// </summary>
        public void Stop()
        {
            if (f_isAlive()) { m_runner.Stop(); }
        }

        /// <summary>
        /// Adds new task to the queue. It is allowed if the current task is alive.
        /// </summary>
        public void ContinueWith(IEnumerator routine)
        {
            if (f_isAlive())
            {
                m_runner.Add(routine);
                m_runner.StartRunning();
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
            return m_runner != null && m_runner.ID == m_id;
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
            return Helper.GetHashCode(m_id.GetHashCode(), m_runner.GetHashCode());
        }

        public static bool operator ==(TaskInfo a, TaskInfo b)
        {
            return a.m_id == b.m_id && a.m_runner == b.m_runner;
        }

        public static bool operator !=(TaskInfo a, TaskInfo b)
        {
            return a.m_id != b.m_id || a.m_runner != b.m_runner;
        }
    }
}
