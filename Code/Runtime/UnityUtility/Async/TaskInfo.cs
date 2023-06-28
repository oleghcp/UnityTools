using System;
using System.Collections;

namespace UnityUtility.Async
{
    public readonly struct TaskResult
    {
        public object Result { get; }
        public bool Successful { get; }

        internal TaskResult(object result, bool successful)
        {
            Result = result;
            Successful = successful;
        }
    }

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

        object IEnumerator.Current => null;

        internal TaskInfo(TaskRunner runner)
        {
            _id = (_task = runner).Id;
        }

        public void AddComleteListener(Action<TaskResult> onComplete)
        {
            if (IsAliveInternal())
                _task.OnCompleted_Event += onComplete;
            else
                onComplete(default);
        }

        /// <summary>
        /// Stops the task and marks it as non-alive.
        /// </summary>
        public void Stop()
        {
            if (IsAliveInternal())
                _task.Stop();
        }

        #region Regular stuff
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

        public static bool operator ==(TaskInfo a, TaskInfo b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TaskInfo a, TaskInfo b)
        {
            return !a.Equals(b);
        }
        #endregion

        #region IEnumerator implementation
        bool IEnumerator.MoveNext()
        {
            return IsAliveInternal();
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }
        #endregion

        private bool IsAliveInternal()
        {
            return _task != null && _task.Id == _id;
        }
    }
}
