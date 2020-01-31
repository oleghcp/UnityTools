using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UU.Async
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TaskInfo : IEquatable<TaskInfo>
    {
        private readonly uint ID;
        internal RoutineExecutor Runner;

        /// <summary>
        /// Provides task ID.
        /// </summary>
        public uint TaskId
        {
            get { return ID; }
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
            get { return f_isAlive() && Runner.IsPaused; }
        }

        internal TaskInfo(RoutineExecutor runner)
        {
            Runner = runner;
            ID = runner.ID;
        }

        /// <summary>
        /// Pauses the task.
        /// </summary>
        public void Pause()
        {
            if (f_isAlive()) { Runner.Pause(); }
        }

        /// <summary>
        /// Resumes paused task.
        /// </summary>
        public void Resume()
        {
            if (f_isAlive()) { Runner.Resume(); }
        }

        /// <summary>
        /// Stops the task and marks it as non-alive.
        /// </summary>
        public void Stop()
        {
            if (f_isAlive()) { Runner.Stop(); }
        }

        /// <summary>
        /// Adds new task to the queue. It is allowed if the current task is alive.
        /// </summary>
        public void ContinueWith(IEnumerator routine)
        {
            if (f_isAlive())
            {
                Runner.Add(routine);
                Runner.StartRunning();
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
            return Runner != null && Runner.ID == ID;
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is TaskInfo && this == (TaskInfo)obj;
        }

        public bool Equals(TaskInfo other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(ID.GetHashCode(), Runner.GetHashCode());
        }

        public static bool operator ==(TaskInfo a, TaskInfo b)
        {
            return a.ID == b.ID && a.Runner == b.Runner;
        }

        public static bool operator !=(TaskInfo a, TaskInfo b)
        {
            return a.ID != b.ID || a.Runner != b.Runner;
        }
    }
}
