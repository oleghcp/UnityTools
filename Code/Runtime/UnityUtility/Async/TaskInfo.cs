using System;
using System.Collections;
using System.Threading;

namespace UnityUtility.Async
{
    public readonly struct TaskInfo : IEquatable<TaskInfo>, IEnumerator
    {
        private readonly long _id;
        private readonly TaskRunner _task;

        /// <summary>
        /// Provides task ID.
        /// </summary>
        public long TaskId => _id;

        /// <summary>
        /// Returns true if the task is alive. The task is alive while it runs.
        /// </summary>
        public bool IsAlive => IsAliveInternal();

        public bool IsPaused => IsAliveInternal() && _task.IsPaused;

        object IEnumerator.Current => null;

        internal TaskInfo(TaskRunner runner)
        {
            _id = (_task = runner).Id;
        }

        /// <summary>
        /// Creates a continuation that executes when the target task completes.
        /// </summary>
        public TaskInfo ContinueWith(IEnumerator routine, in CancellationToken token = default)
        {
            if (IsAliveInternal())
                return _task.ContinueWith(routine, token);

            if (_task != null)
                return _task.Owner.GetRunner().RunAsync(routine, token);

            return TaskSystem.StartAsyncLocally(routine, token);
        }

        /// <summary>
        /// Pauses the task.
        /// </summary>
        public void Pause()
        {
            if (IsAliveInternal())
                _task.Pause();
        }

        /// <summary>
        /// Resumes paused task.
        /// </summary>
        public void Resume()
        {
            if (IsAliveInternal())
                _task.Resume();
        }

        /// <summary>
        /// Stops the task and marks it as non-alive.
        /// </summary>
        public void Stop()
        {
            if (IsAliveInternal())
                _task.Stop();
        }

        private bool IsAliveInternal()
        {
            return _task != null && _task.Id == _id;
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is TaskInfo taskInfo && Equals(taskInfo);
        }

        public bool Equals(TaskInfo other)
        {
            return other._id == _id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        bool IEnumerator.MoveNext()
        {
            return IsAliveInternal();
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(TaskInfo a, TaskInfo b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TaskInfo a, TaskInfo b)
        {
            return !a.Equals(b);
        }
    }
}
