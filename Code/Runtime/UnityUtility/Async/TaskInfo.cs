using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace UnityUtility.Async
{
    public struct TaskInfo : IEquatable<TaskInfo>
    {
        private readonly long _id;
        private readonly RoutineRunner _task;

        /// <summary>
        /// Provides task ID.
        /// </summary>
        public long TaskId => _id;

        /// <summary>
        /// Returns true if the task is alive. The task is alive while it runs.
        /// </summary>
        public bool IsAlive => IsAliveInternal();

        public bool IsPaused => IsAliveInternal() && _task.IsPaused;

        internal TaskInfo(RoutineRunner runner)
        {
            _id = (_task = runner).Id;
        }

        /// <summary>
        /// Pauses the task.
        /// </summary>
        public void Pause()
        {
            if (IsAliveInternal()) { _task.Pause(); }
        }

        /// <summary>
        /// Resumes paused task.
        /// </summary>
        public void Resume()
        {
            if (IsAliveInternal()) { _task.Resume(); }
        }

        /// <summary>
        /// Skips the current coroutine at the queue.
        /// </summary>
        public void SkipCurrent()
        {
            if (IsAliveInternal()) { _task.SkipCurrent(); }
        }

        /// <summary>
        /// Stops the task and marks it as non-alive.
        /// </summary>
        public void Stop()
        {
            if (IsAliveInternal()) { _task.Stop(); }
        }

        /// <summary>
        /// Adds new routine to the queue. It is allowed if the current task is alive.
        /// </summary>
        public void Add(IEnumerator routine)
        {
            if (IsAliveInternal())
            {
                _task.Add(routine);
                return;
            }

            throw new InvalidOperationException("Task is not alive.");
        }

        // - - //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsAliveInternal()
        {
            return UnityObjectUtility.IsAlive(_task) && _task.Id == _id;
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is TaskInfo taskInfo && this == taskInfo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(TaskInfo other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(TaskInfo a, TaskInfo b)
        {
            return a._id == b._id;
        }

        public static bool operator !=(TaskInfo a, TaskInfo b)
        {
            return a._id != b._id;
        }
    }
}
